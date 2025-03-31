using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Questao5.Controllers.Models
{
    public class ResponseExamples : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            AddGetBalanceExample(operation, context);
            AddPostTransactionExample(operation, context);
        }

        private void AddGetBalanceExample(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.RelativePath.Equals("api/CurrentAccount/{id}", StringComparison.OrdinalIgnoreCase))
            {
                operation.Responses["200"].Content["application/json"].Example = new OpenApiObject
                {
                    ["status"] = new OpenApiString("SUCCESS"),
                    ["message"] = new OpenApiNull(),
                    ["data"] = new OpenApiObject
                    {
                        ["accountNumber"] = new OpenApiInteger(12345),
                        ["name"] = new OpenApiString("Lorem Ipsum"),
                        ["queryTimestamp"] = new OpenApiDateTime(DateTime.MinValue),
                        ["balance"] = new OpenApiString("R$ 123,45")
                    },
                    ["errorType"] = new OpenApiNull()
                };

                operation.Responses["400"].Content["application/json"].Example = new OpenApiObject
                {
                    ["Status"] = new OpenApiString("ERROR"),
                    ["Message"] = new OpenApiString("Descrição do erro"),
                    ["Data"] = new OpenApiNull(),
                    ["ErrorType"] = new OpenApiString("INVALID_VALUE")
                };
            }
        }

        private void AddPostTransactionExample(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.RelativePath.Equals("api/CurrentAccount/Transaction", StringComparison.OrdinalIgnoreCase))
            {
                operation.Responses["200"].Content["application/json"].Example = new OpenApiObject
                {
                    ["status"] = new OpenApiString("SUCCESS"),
                    ["message"] = new OpenApiNull(),
                    ["data"] = new OpenApiObject
                    {
                        ["id"] = new OpenApiString("468ba66c-b7d9-4fdf-ae42-be8c15d75566")
                    },
                    ["errorType"] = new OpenApiNull()
                };

                operation.Responses["400"].Content["application/json"].Example = new OpenApiObject
                {
                    ["Status"] = new OpenApiString("ERROR"),
                    ["Message"] = new OpenApiString("Descrição do erro"),
                    ["Data"] = new OpenApiNull(),
                    ["ErrorType"] = new OpenApiString("INVALID_VALUE")
                };
            }
        }

    }
}
