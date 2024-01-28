﻿using WISHLIST.Models.Domain;
using WISHLIST.Models.DTO;

namespace WISHLIST.Repositories.Abstract
{
    public interface IWishlistService
    {
        public Task<List<WishlistModel>> GetAuthorWishlistListAsync(string Username);

        public Task<WishlistModel> GetCurrentWishListAsync(string WishlistId);

        public Task<StatusModel> CreateWishlistAsync(CreateWishlistModel model, string username);

        public Task<StatusModel> UpdateCurrentWishlistAsync(CreateWishlistModel model);

        public Task<StatusModel> DeleteCurrentWishlistAsync(string WishlistId);

    }
}