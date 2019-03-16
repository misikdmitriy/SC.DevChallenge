using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SC.DevChallenge.Api.MediatorRequests
{
    public class SimpleRequest : IRequest<string>
    {
    }

    public class SimpleResultHandler : IRequestHandler<SimpleRequest, string>
    {
        public Task<string> Handle(SimpleRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.Empty);
        }
    }
}
