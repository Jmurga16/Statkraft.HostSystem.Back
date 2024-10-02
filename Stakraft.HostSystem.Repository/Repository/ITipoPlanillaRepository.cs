using Stakraft.HostSystem.Repository.Entity;

namespace Stakraft.HostSystem.Repository.Repository
{
    public interface ITipoPlanillaRepository
    {
        List<TipoPlanilla> Listar(string banco);
        void Guardar(TipoPlanilla tipoPlanilla);
        void Actualizar(TipoPlanilla tipoPlanilla);
        bool Inactivar(int idTipoPlanilla, string usuario);
        bool Existe(string nombre, int? idTipoPlanilla);
    }
}
