using Entity.Test;
using Model.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IGeminiService
    {
        Task<SubmitTestDto> ScoreAndExplain(SubmitTestDto model);
        //Task<SubmitTestDto> ScoreAndExplainSpeaking(SubmitTestDto model);
        Task ExplainListeningAndReading(Guid partId, int skillType);
        Task<SpeakingResponseDto> ScoreSpeaking(SpeakingModel model);
    }
}
