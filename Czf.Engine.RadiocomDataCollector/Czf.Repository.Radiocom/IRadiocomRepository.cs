using System.Collections.Generic;
using System.Threading.Tasks;


namespace Czf.Repository.Radiocom
{
    public interface IRadiocomRepository
    {
        Task<int> ProcessRawOccurrances(IEnumerable<RawArtistWorkStationOccurrence> occurrences);
    }
}

