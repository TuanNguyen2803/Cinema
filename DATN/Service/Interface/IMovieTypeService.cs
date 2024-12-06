using DATN.Payload.DTO;
using DATN.Payload.Request.MovieTypeRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IMovieTypeService
    {
        ResponseObject<MovieTypeDTO> Create(ThemMovieTypeRequest request);
        ResponseObject<MovieTypeDTO> Update(SuaMovieTypeRequest request);
        ResponseObject<MovieTypeDTO> Delete(int movieid);
        IQueryable<MovieTypeDTO> GetAll();
    }
}
