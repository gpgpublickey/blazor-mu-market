using mumarket.DataContracts.Request;

namespace mumarket.Models.Commands
{
    public class CommandsChainFactory
    {
        public BaseCommand CreateGeneralChain(CommandRequest request, MuMarketDbContext _db)
        {
            var deobfuscateCmd = new DeobCommand(request, _db);
            var pluskarmaCmd = new PositiveKarmaCommand(request, _db);
            deobfuscateCmd.SetNext(pluskarmaCmd);

            return deobfuscateCmd;
        }
    }
}

/*  
 '<div class="_akbu"><span dir="auto" aria-label="" class="_ao3e selectable-text copyable-text" style="min-height: 0px;"><span>/+1 <span role="button" tabindex="0" class=""><span dir="auto" data-jid="5491136632684@c.us" data-display="Andru" class="x1ypdohk x1a06ls3 _ao3e selectable-text select-all copyable-text" data-plain-text="@Andru" data-app-text-template="​5491136632684@c.us​">@<span dir="ltr">Andru</span></span></span></span></span><span class=""><span class="x3nfvp2 xxymvpz xlshs6z xqtp20y xexx8yu x150jy0e x18d9i69 x1e558r4 x12lo8hy x152skdk" aria-hidden="true"><span class="x1c4vz4f x2lah0s xn6xy2s"></span><span class="x1c4vz4f x2lah0s">17:10</span></span></span></div>'
 
 */