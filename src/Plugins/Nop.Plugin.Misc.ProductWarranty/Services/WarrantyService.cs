// WarrantyService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.ProductWarranty.Domain;

namespace Nop.Plugin.Misc.ProductWarranty.Services
{
    /// <summary>
    /// Warranty service
    /// </summary>
    public class WarrantyService : IWarrantyService
    {
        #region Fields

        private readonly IRepository<WarrantyCategoryRecord> _warrantyCategoryRepository;
        private readonly IRepository<ProductWarrantyMappingRecord> _productWarrantyMappingRepository;

        #endregion

        #region Ctor

        public WarrantyService(
            IRepository<WarrantyCategoryRecord> warrantyCategoryRepository,
            IRepository<ProductWarrantyMappingRecord> productWarrantyMappingRepository)
        {
            _warrantyCategoryRepository = warrantyCategoryRepository;
            _productWarrantyMappingRepository = productWarrantyMappingRepository;
        }

        #endregion

        #region Methods

        #region Warranty categories

        /// <summary>
        /// Gets a warranty category by identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warranty category
        /// </returns>
        public virtual async Task<WarrantyCategoryRecord> GetWarrantyCategoryByIdAsync(int categoryId)
        {
            return await _warrantyCategoryRepository.GetByIdAsync(categoryId);
        }

        /// <summary>
        /// Gets all warranty categories
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warranty categories
        /// </returns>
        public virtual async Task<IList<WarrantyCategoryRecord>> GetAllWarrantyCategoriesAsync(bool showHidden = false)
        {
            var query = from wc in _warrantyCategoryRepository.Table
                        where showHidden || wc.Published
                        orderby wc.DisplayOrder, wc.Name
                        select wc;

            return await query.ToListAsync();
        }

        /// <summary>
        /// Inserts a warranty category
        /// </summary>
        /// <param name="category">Warranty category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertWarrantyCategoryAsync(WarrantyCategoryRecord category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            category.CreatedOnUtc = DateTime.UtcNow;
            category.UpdatedOnUtc = DateTime.UtcNow;

            await _warrantyCategoryRepository.InsertAsync(category);
        }

        /// <summary>
        /// Updates the warranty category
        /// </summary>
        /// <param name="category">Warranty category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateWarrantyCategoryAsync(WarrantyCategoryRecord category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            category.UpdatedOnUtc = DateTime.UtcNow;

            await _warrantyCategoryRepository.UpdateAsync(category);
        }

        /// <summary>
        /// Deletes the warranty category
        /// </summary>
        /// <param name="category">Warranty category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteWarrantyCategoryAsync(WarrantyCategoryRecord category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            await _warrantyCategoryRepository.DeleteAsync(category);
        }

        #endregion

        #region Product warranty mappings
        
        /// <summary>
        /// Gets all product warranty mappings
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product warranty mappings
        /// </returns>
        public virtual async Task<IList<ProductWarrantyMappingRecord>> GetAllProductWarrantyMappingsAsync(bool showHidden = false)
        {
            var query = from pm in _productWarrantyMappingRepository.Table
                where showHidden || pm.IsActive
                orderby pm.DisplayOrder
                select pm;

            return await query.ToListAsync();
        }

        /// <summary>
        /// Gets a product warranty mapping by identifier
        /// </summary>
        /// <param name="mappingId">Mapping identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product warranty mapping
        /// </returns>
        public virtual async Task<ProductWarrantyMappingRecord> GetProductWarrantyMappingByIdAsync(int mappingId)
        {
            return await _productWarrantyMappingRepository.GetByIdAsync(mappingId);
        }

        /// <summary>
        /// Gets product warranty mappings by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product warranty mappings
        /// </returns>
        public virtual async Task<IList<ProductWarrantyMappingRecord>> GetProductWarrantyMappingsByProductIdAsync(int productId, bool showHidden = false)
        {
            var query = from pm in _productWarrantyMappingRepository.Table
                        where pm.ProductId == productId && (showHidden || pm.IsActive)
                        orderby pm.DisplayOrder
                        select pm;

            return await query.ToListAsync();
        }

        /// <summary>
        /// Gets product warranty mappings by warranty category identifier
        /// </summary>
        /// <param name="categorytId">Warranty category identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product warranty mappings
        /// </returns>
        public virtual async Task<IList<ProductWarrantyMappingRecord>> GetProductWarrantyMappingsByCategoryIdAsync(int categorytId)
        {
            var query = from pm in _productWarrantyMappingRepository.Table
                        where pm.WarrantyCategoryId == categorytId
                        orderby pm.DisplayOrder
                        select pm;

            return await query.ToListAsync();
        }

        /// <summary>
        /// Inserts a product warranty mapping
        /// </summary>
        /// <param name="mapping">Product warranty mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertProductWarrantyMappingAsync(ProductWarrantyMappingRecord mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));

            await _productWarrantyMappingRepository.InsertAsync(mapping);
        }

        /// <summary>
        /// Updates the product warranty mapping
        /// </summary>
        /// <param name="mapping">Product warranty mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateProductWarrantyMappingAsync(ProductWarrantyMappingRecord mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));

            await _productWarrantyMappingRepository.UpdateAsync(mapping);
        }

        /// <summary>
        /// Deletes the product warranty mapping
        /// </summary>
        /// <param name="mapping">Product warranty mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteProductWarrantyMappingAsync(ProductWarrantyMappingRecord mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));

            await _productWarrantyMappingRepository.DeleteAsync(mapping);
        }

        #endregion

        #endregion
    }
}