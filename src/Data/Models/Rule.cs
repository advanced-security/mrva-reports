namespace MRVA.Reports.Data.Models;

public class Rule
{
    public string Message { get; set; }
    
    public int RowId { get; set; }
    
    public string RuleId { get; set; }

    public override int GetHashCode() => HashCode.Combine(RowId);
}