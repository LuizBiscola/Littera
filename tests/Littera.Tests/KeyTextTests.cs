using System.Windows.Forms;
using Littera.Services;

namespace Littera.Tests;

public class KeyTextTests
{
    [Theory]
    [InlineData("Ctrl+Shift+F", Keys.Control | Keys.Shift | Keys.F)]
    [InlineData("ctrl + s", Keys.Control | Keys.S)]
    [InlineData("F11", Keys.F11)]
    [InlineData("Alt+Z", Keys.Alt | Keys.Z)]
    [InlineData("Ctrl+0", Keys.Control | Keys.D0)]
    [InlineData("Ctrl+Plus", Keys.Control | Keys.Oemplus)]
    [InlineData("Ctrl++", Keys.Control | Keys.Oemplus)]
    [InlineData("Ctrl+Comma", Keys.Control | Keys.Oemcomma)]
    [InlineData("Shift+Tab", Keys.Shift | Keys.Tab)]
    public void Entende_atalhos_escritos(string text, Keys expected)
    {
        Assert.Equal(expected, KeyText.Parse(text));
    }

    [Theory]
    [InlineData("Ctrl+")]
    [InlineData("Ctrl+Shift")]
    [InlineData("Ctrl+Banana")]
    [InlineData("")]
    public void Rejeita_atalhos_invalidos(string text)
    {
        Assert.Null(KeyText.Parse(text));
    }

    [Fact]
    public void Aceita_varios_atalhos_separados_por_ponto_e_virgula()
    {
        Assert.Equal([Keys.Control | Keys.Oemplus, Keys.Control | Keys.Add], KeyText.ParseAll("Ctrl+Plus; Ctrl+Add"));
    }
}
