﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Store.PartnerCenter.PowerShell.Commands
{
    using System.Management.Automation;
    using Agreements;
    using Models.Agreements;
    using Models.Authentication;

    [Cmdlet(VerbsCommon.Get, "PartnerAgreementDocument")]
    [OutputType(typeof(PSAgreementDocument))]
    public class GetPartnerAgreementDocument : PartnerAsyncCmdlet
    {
        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        [Parameter(HelpMessage = "The country of the agreement document.", Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the language and locale of the agreement document.
        /// </summary>
        [Parameter(HelpMessage = "The language and locale of the agreement document.", Mandatory = false)]
        [ValidateNotNullOrEmpty]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets unique identifier of the agreement type. 
        /// </summary>
        [Parameter(HelpMessage = "The unique identifier of the agreement type.", Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string TemplateId { get; set; }

        /// <summary>
        /// Executes the operations associated with the cmdlet.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            Scheduler.RunTask(async () =>
            {
                IPartner partner = await PartnerSession.Instance.ClientFactory.CreatePartnerOperationsAsync(CorrelationId, CancellationToken).ConfigureAwait(false);
                IAgreementDocument operation = partner.AgreementTemplates.ById(TemplateId).Document;

                if (!string.IsNullOrEmpty(Country))
                {
                    operation = operation.ByCountry(Country);
                }

                if (!string.IsNullOrEmpty(Language))
                {
                    operation = operation.ByLanguage(Language);
                }

                WriteObject(new PSAgreementDocument(await operation.GetAsync(CancellationToken).ConfigureAwait(false)));
            }, true);
        }
    }
}