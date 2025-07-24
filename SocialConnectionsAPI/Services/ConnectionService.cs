using Microsoft.EntityFrameworkCore;
using SocialConnectionsAPI.Data;
using SocialConnectionsAPI.DTOs;
using SocialConnectionsAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialConnectionsAPI.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService; // Inject UserService to check user existence

        public ConnectionService(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<ServiceResult<ConnectionStatusResponse>> CreateConnectionAsync(ConnectionRequest request)
        {
            // 1. Validate user existence
            if (!await _userService.UserExistsAsync(request.User1StrId) || !await _userService.UserExistsAsync(request.User2StrId))
            {
                return ServiceResult<ConnectionStatusResponse>.Failure("One or both users not found.", "users_not_found");
            }

            // 2. Enforce consistent ordering (lexicographically smaller first)
            string orderedUser1 = string.Compare(request.User1StrId, request.User2StrId) < 0 ? request.User1StrId : request.User2StrId;
            string orderedUser2 = string.Compare(request.User1StrId, request.User2StrId) < 0 ? request.User2StrId : request.User1StrId;

            // Prevent self-connection
            if (orderedUser1 == orderedUser2)
            {
                return ServiceResult<ConnectionStatusResponse>.Failure("Cannot connect to self.", "self_connection_invalid");
            }

            // 3. Check if connection already exists
            if (await _context.Connections.AnyAsync(c => c.User1StrId == orderedUser1 && c.User2StrId == orderedUser2))
            {
                return ServiceResult<ConnectionStatusResponse>.Failure("Connection already exists.", "connection_exists");
            }

            var connection = new Connection
            {
                User1StrId = orderedUser1,
                User2StrId = orderedUser2
            };

            _context.Connections.Add(connection);
            await _context.SaveChangesAsync();

            return ServiceResult<ConnectionStatusResponse>.Success(new ConnectionStatusResponse { Status = "connection_added" });
        }

        public async Task<ServiceResult<ConnectionStatusResponse>> RemoveConnectionAsync(ConnectionRequest request)
        {
            // 1. Validate user existence
            if (!await _userService.UserExistsAsync(request.User1StrId) || !await _userService.UserExistsAsync(request.User2StrId))
            {
                return ServiceResult<ConnectionStatusResponse>.Failure("One or both users not found.", "users_not_found");
            }

            // 2. Enforce consistent ordering for lookup
            string orderedUser1 = string.Compare(request.User1StrId, request.User2StrId) < 0 ? request.User1StrId : request.User2StrId;
            string orderedUser2 = string.Compare(request.User1StrId, request.User2StrId) < 0 ? request.User2StrId : request.User1StrId;

            // 3. Find the connection
            var connection = await _context.Connections
                .FirstOrDefaultAsync(c => c.User1StrId == orderedUser1 && c.User2StrId == orderedUser2);

            if (connection == null)
            {
                return ServiceResult<ConnectionStatusResponse>.Failure("Connection does not exist.", "not_connected");
            }

            // 4. Remove the connection
            _context.Connections.Remove(connection);
            await _context.SaveChangesAsync();

            return ServiceResult<ConnectionStatusResponse>.Success(new ConnectionStatusResponse { Status = "connection_removed" });
        }

        public async Task<ServiceResult<DegreeSeparationResponse>> GetDegreeOfSeparationAsync(string fromUserStrId, string toUserStrId)
        {
            // 1. Validate user existence
            if (!await _userService.UserExistsAsync(fromUserStrId) || !await _userService.UserExistsAsync(toUserStrId))
            {
                return ServiceResult<DegreeSeparationResponse>.Failure("One or both users not found.", "users_not_found");
            }

            // Handle self-connection as 0 degree
            if (fromUserStrId == toUserStrId)
            {
                return ServiceResult<DegreeSeparationResponse>.Success(new DegreeSeparationResponse { Degree = 0 });
            }

            // 2. Build Adjacency List (in-memory graph representation for BFS)
            // This is crucial for efficient BFS traversal.
            // We fetch all connections to build the graph. For very large datasets,
            // this might need optimization (e.g., fetching only relevant subgraph).
            var allConnections = await _context.Connections.ToListAsync();
            var adjacencyList = new Dictionary<string, List<string>>();

            foreach (var conn in allConnections)
            {
                if (!adjacencyList.ContainsKey(conn.User1StrId))
                    adjacencyList[conn.User1StrId] = new List<string>();
                if (!adjacencyList.ContainsKey(conn.User2StrId))
                    adjacencyList[conn.User2StrId] = new List<string>();

                adjacencyList[conn.User1StrId].Add(conn.User2StrId);
                adjacencyList[conn.User2StrId].Add(conn.User1StrId); // Mutual connection
            }

            // Ensure start and end nodes exist in the graph (i.e., have connections)
            if (!adjacencyList.ContainsKey(fromUserStrId) || !adjacencyList.ContainsKey(toUserStrId))
            {
                // If a user has no connections, they won't be in the adjacency list.
                // If target user isn't in graph, means they are not connected to anyone.
                return ServiceResult<DegreeSeparationResponse>.Success(new DegreeSeparationResponse { Degree = -1, Message = "not_connected" });
            }


            // 3. Implement Breadth-First Search (BFS)
            var queue = new Queue<Tuple<string, int>>(); // (UserStrId, Degree)
            var visited = new HashSet<string>();

            queue.Enqueue(Tuple.Create(fromUserStrId, 0));
            visited.Add(fromUserStrId);

            while (queue.Any())
            {
                var current = queue.Dequeue();
                string currentUser = current.Item1;
                int currentDegree = current.Item2;

                if (currentUser == toUserStrId)
                {
                    return ServiceResult<DegreeSeparationResponse>.Success(new DegreeSeparationResponse { Degree = currentDegree });
                }

                // Explore neighbors
                if (adjacencyList.TryGetValue(currentUser, out var neighbors))
                {
                    foreach (var neighbor in neighbors)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            queue.Enqueue(Tuple.Create(neighbor, currentDegree + 1));
                        }
                    }
                }
            }

            // If BFS completes and target user is not found
            return ServiceResult<DegreeSeparationResponse>.Success(new DegreeSeparationResponse { Degree = -1, Message = "not_connected" });
        }
    }
}
