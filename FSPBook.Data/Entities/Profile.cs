using System.ComponentModel.DataAnnotations.Schema;

namespace FSPBook.Data.Entities;
public class Profile
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string JobTitle { get; set; }

    [NotMapped]
    public string FullName => FirstName + " " + LastName;
}
