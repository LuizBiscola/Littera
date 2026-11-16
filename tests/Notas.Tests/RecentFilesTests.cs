using Notas.Services;

namespace Notas.Tests;

public class RecentFilesTests
{
    [Fact]
    public void Move_para_o_topo_sem_duplicar_ignorando_maiusculas()
    {
        var recent = new List<string> { @"C:\a.sql", @"C:\b.sql" };
        RecentFiles.Add(recent, @"c:\B.SQL");
        Assert.Equal([@"c:\B.SQL", @"C:\a.sql"], recent);
    }

    [Fact]
    public void Mantem_no_maximo_o_limite()
    {
        var recent = new List<string>();
        for (int i = 0; i < RecentFiles.Max + 5; i++)
            RecentFiles.Add(recent, $@"C:\{i}.sql");

        Assert.Equal(RecentFiles.Max, recent.Count);
        Assert.Equal($@"C:\{RecentFiles.Max + 4}.sql", recent[0]);
    }
}
