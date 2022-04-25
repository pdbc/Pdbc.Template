using Aertssen.Framework.Audit.Contracts.Dto;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace MyTemplate.Api.Extensions
{
    public static class ODataExtensions
    {
        public static ODataOptions ConfigureODataOptions(this ODataOptions options)
        {            
            // Enable needed query options
            options.Expand().Select().OrderBy().Count().Filter().SetMaxTop(100);
            // Add an edm model to the specified route
            options.AddRouteComponents(routePrefix: "odata", model: GetEdmModel());
            return options;
        }

        /// <summary>
        /// Builds the Entity Data Model used for OData
        /// You can see the generated schema by navigation to https://localhost:44348/odata/$metadata
        /// </summary>
        /// <returns></returns>
        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<AuditInfo>("AuditRecords").EntityType.HasKey(k => k.AreaId);
            

            builder.EnableLowerCamelCase();
             
            return builder.GetEdmModel();
        }
    }
}
