﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using MovieRank.Libs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieRank.Libs.Repositories
{
    public class MovieRankRepository : IMovieRankRepository
    {
        private readonly DynamoDBContext _context;

        public MovieRankRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _context = new DynamoDBContext(dynamoDbClient);
        }

        public async Task<IEnumerable<MovieDb>> GetAllItems()
        {
            return await _context.ScanAsync<MovieDb>(new List<ScanCondition>()).GetRemainingAsync();
        }

        public async Task<MovieDb> GetMovie(int userId, string movieName)
        {
            return await _context.LoadAsync<MovieDb>(userId, movieName);
        }

        public async Task<IEnumerable<MovieDb>> GetUsersRankedMoviesByMovieTitle(int userId, string movieName)
        {
            var config = new DynamoDBOperationConfig
            {
                QueryFilter = new List<ScanCondition>
                {
                    new ScanCondition("MovieName", Amazon.DynamoDBv2.DocumentModel.ScanOperator.BeginsWith, movieName)
                }
            };

            return await _context.QueryAsync<MovieDb>(userId, config).GetRemainingAsync();
        }

        public async Task AddMovie(MovieDb movieDb)
        {
            await _context.SaveAsync(movieDb);
        }

        public async Task UpdateMovie(MovieDb request)
        {
            await _context.SaveAsync(request);
        }
    }
}
