namespace JDM.Playground.StringTokenizer
{
    public interface ITokenFormatter<TDataSourceType>
    {
        string Format(TDataSourceType dataToFormat, string formatString);
    }
}