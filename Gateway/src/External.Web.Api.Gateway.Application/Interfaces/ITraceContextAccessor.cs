using External.Web.Api.Gateway.Domain.Models;

namespace External.Web.Api.Gateway.Application.Interfaces;

public interface ITraceContextAccessor
{
    TraceContext? Current { get; set; }
}
