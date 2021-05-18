using AutoWrapper;

namespace TASI.Backend.Infrastructure.Configs
{
    public class MapResponseObject
    {
        [AutoWrapperPropertyMap(Prop.Result)]
        public object Data { get; set; }
    }
}
