
// IWarrantyService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Misc.ProductWarranty.Domain;

namespace Nop.Plugin.Misc.ProductWarranty.Services
{
    /// <summary>
    /// Warranty service interface
    /// </summary>
    public interface IWarrantyService
    {
        #region Warranty categories

        /// <summary>
        /// Gets a warranty category by identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warranty category
        /// </returns>
        Task<WarrantyCategoryRecord> GetWarrantyCategoryByIdAsync(int categoryId);

        /// <summary>
        /// Gets all warranty categories
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the warranty categories
        /// </returns>
        Task<IList<WarrantyCategoryRecord>> GetAllWarrantyCategoriesAsync(bool showHidden = false);

        /// <summary>
        /// Inserts a warranty category
        /// </summary>
        /// <param name="category">Warranty category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertWarrantyCategoryAsync(WarrantyCategoryRecord category);

        /// <summary>
        /// Updates the warranty category
        /// </summary>
        /// <param name="category">Warranty category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateWarrantyCategoryAsync(WarrantyCategoryRecord category);

        /// <summary>
        /// Deletes the warranty category
        /// </summary>
        /// <param name="category">Warranty category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteWarrantyCategoryAsync(WarrantyCategoryRecord category);

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
        Task<IList<ProductWarrantyMappingRecord>> GetAllProductWarrantyMappingsAsync(bool showHidden = false);

        /// <summary>
        /// Gets a product warranty mapping by identifier
        /// </summary>
        /// <param name="mappingId">Mapping identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product warranty mapping
        /// </returns>
        Task<ProductWarrantyMappingRecord> GetProductWarrantyMappingByIdAsync(int mappingId);

        /// <summary>
        /// Gets product warranty mappings by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product warranty mappings
        /// </returns>
        Task<IList<ProductWarrantyMappingRecord>> GetProductWarrantyMappingsByProductIdAsync(int productId, bool showHidden = false);

        /// <summary>
        /// Gets product warranty mappings by warranty category identifier
        /// </summary>
        /// <param name="categorytId">Warranty category identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product warranty mappings
        /// </returns>
        Task<IList<ProductWarrantyMappingRecord>> GetProductWarrantyMappingsByCategoryIdAsync(int categorytId);

        /// <summary>
        /// Inserts a product warranty mapping
        /// </summary>
        /// <param name="mapping">Product warranty mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertProductWarrantyMappingAsync(ProductWarrantyMappingRecord mapping);

        /// <summary>
        /// Updates the product warranty mapping
        /// </summary>
        /// <param name="mapping">Product warranty mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateProductWarrantyMappingAsync(ProductWarrantyMappingRecord mapping);

        /// <summary>
        /// Deletes the product warranty mapping
        /// </summary>
        /// <param name="mapping">Product warranty mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteProductWarrantyMappingAsync(ProductWarrantyMappingRecord mapping);

        #endregion
    }
}