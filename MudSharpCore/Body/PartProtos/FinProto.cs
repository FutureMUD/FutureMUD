
namespace MudSharp.Body.PartProtos;

public class FinProto : DrapeableBodypartProto
{
    public FinProto(DrapeableBodypartProto rhs, string newName) : base(rhs, newName)
    {
    }

    public FinProto(Models.BodypartProto proto, IFuturemud game) : base(proto, game)
    {
    }

    public override BodypartTypeEnum BodypartType => BodypartTypeEnum.Fin;
}