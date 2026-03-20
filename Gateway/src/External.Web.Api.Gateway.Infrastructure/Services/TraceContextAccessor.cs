using External.Web.Api.Gateway.Application.Interfaces;
using External.Web.Api.Gateway.Domain.Models;

namespace External.Web.Api.Gateway.Infrastructure.Services;

public sealed class TraceContextAccessor : ITraceContextAccessor
{
    private static readonly AsyncLocal<TraceContextHolder> Context = new();

    public TraceContext? Current
    {
        get => Context.Value?.Context;
        set
        {
            var holder = Context.Value;
            if (holder is not null)
            {
                holder.Context = null;
            }

            if (value is not null)
            {
                Context.Value = new TraceContextHolder { Context = value };
            }
        }
    }

    private sealed class TraceContextHolder
    {
        public TraceContext? Context { get; set; }
    }
}
