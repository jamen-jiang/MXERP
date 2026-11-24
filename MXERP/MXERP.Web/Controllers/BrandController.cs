using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MXERP.Domain.Brands;

namespace MXERP.Web.Controllers;

/// <summary>品牌接口: 验证领域层(聚合根/工厂/领域服务/仓储)与 EF 映射全链路。</summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public sealed class BrandController : ControllerBase
{
    private readonly IBrandRepository _repository;
    private readonly BrandDomainService _domainService;

    /// <summary>构造器</summary>
    /// <param name="repository">品牌仓储</param>
    /// <param name="domainService">品牌领域服务</param>
    public BrandController(IBrandRepository repository, BrandDomainService domainService)
    {
        _repository = repository;
        _domainService = domainService;
    }

    /// <summary>全部品牌(软删除的自动过滤掉)</summary>
    [HttpGet]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        return Ok(await _repository.GetListAsync(cancellationToken: cancellationToken));
    }

    /// <summary>按代码取品牌</summary>
    [HttpGet("{code}")]
    public async Task<IActionResult> GetByCode(string code, CancellationToken cancellationToken)
    {
        Brand? brand = await _repository.GetByCodeAsync(code, cancellationToken);
        return brand is null ? NotFound() : Ok(brand);
    }

    /// <summary>导入生成: 不存在则新建, 存在则修改(对应 Java 的 importGenerate)。Id 由数据库生成, 新建时为 0。</summary>
    [HttpPost("import")]
    public async Task<IActionResult> Import([FromBody] ImportBrandInput input, CancellationToken cancellationToken)
    {
        Brand brand = await _domainService.ImportGenerateAsync(input.Code, input.Name, input.Remark, cancellationToken);

        if (brand.Id == 0)
        {
            await _repository.InsertAsync(brand, autoSave: true, cancellationToken: cancellationToken);
        }
        else
        {
            await _repository.UpdateAsync(brand, autoSave: true, cancellationToken: cancellationToken);
        }

        return Ok(brand);
    }

    /// <summary>删除品牌(软删除: 置 IsDeleted=1, 行仍在)</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(id, autoSave: true, cancellationToken: cancellationToken);
        return NoContent();
    }
}

/// <summary>导入品牌的入参</summary>
public sealed class ImportBrandInput
{
    /// <summary>品牌代码</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>品牌名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}
