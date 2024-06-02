﻿// SvgToXaml
// Copyright (C) 2022-2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using MediatR;

namespace DustInTheWind.SvgToXaml.Application.UseCases.GetOptionsState;

internal class GetOptionsStateUseCase : IRequestHandler<GetOptionsStateRequest, GetOptionsStateResponse>
{
    private readonly ApplicationState applicationState;

    public GetOptionsStateUseCase(ApplicationState applicationState)
    {
        this.applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
    }

    public Task<GetOptionsStateResponse> Handle(GetOptionsStateRequest request, CancellationToken cancellationToken)
    {
        GetOptionsStateResponse response = new()
        {
            ApplyOptimizations = applicationState.ProcessingOptions.OptimizeOutput,
            IgnoredNamespaces = applicationState.ProcessingOptions.IgnoredNamespaces.ToArray()
        };

        return Task.FromResult(response);
    }
}