namespace GlucacxeScadaSystem.Models;

/// <summary>
/// TulingRead.xlsx 表结构
/// </summary>
public class ReadEntity 
{
    public string Module { get; set; }
    public string Cn { get; set; }
    public string En { get; set; }
    public string Address { get; set; }
    public bool Save { get; set; }
    
}

/// <summary>
/// TulingWrite.xlsx 表结构
/// </summary>
public class WriteEntity
{
    public string Module { get; set; }
    public string Cn { get; set; }
    public string En { get; set; }
    public string Address { get; set; }
    public string Save { get; set; }
    public string Default { get; set; }
    public string Unit { get; set; }



}