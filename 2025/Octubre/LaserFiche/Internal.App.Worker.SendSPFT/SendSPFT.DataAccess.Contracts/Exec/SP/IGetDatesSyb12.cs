using System.Data;


using SendSPFT.Business.Models.BD;

namespace SendSPFT.DataAccess.Contracts.Context
{
    public interface IGetDatesSyb12
    {
        Task<GetDatesSyb12ResponseDto> ExecuteSpGetDates();
        GetDatesSyb12ResponseDto MapResult(DataSet ds, string dataOrigin);
    }
}
