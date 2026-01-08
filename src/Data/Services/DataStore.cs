using System.Collections.Immutable;
using MRVA.Reports.Data.Helpers;
using MRVA.Reports.Data.Models;
using SolTechnology.Avro;

namespace MRVA.Reports.Data.Services;

public class DataStore
{
    
    public IReadOnlySet<Rule> RuleSet { get; }
    
    public DataStore()
    {
        var bytes = ResourceHelper.GetResource("rule.avro");
        RuleSet = AvroConvert
            .Deserialize<HashSet<Rule>>(bytes)?
            .ToImmutableHashSet() ?? ImmutableHashSet<Rule>.Empty;
    }

}