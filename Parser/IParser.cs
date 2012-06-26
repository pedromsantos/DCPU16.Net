namespace Parser
{
    using System.Collections.Generic;

    using Model;

    public interface IParser
    {
        IList<Statment> Statments { get; }

        IEnumerable<Statment> Parse();
    }
}