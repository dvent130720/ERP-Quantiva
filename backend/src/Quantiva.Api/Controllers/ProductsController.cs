using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quantiva.Application.Common.Abstractions;
using Quantiva.Application.DTOs;

namespace Quantiva.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/products")]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await productService.GetAllAsync(cancellationToken));

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] UpsertProductDto request, CancellationToken cancellationToken)
        => Ok(await productService.CreateAsync(request, cancellationToken));
}
