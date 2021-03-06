﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Store.PartnerCenter.PowerShell.Commands
{
    using System.Management.Automation;
    using System.Text.RegularExpressions;
    using Azure.Management.Profiles.Subscription;
    using Azure.Management.Profiles.Subscription.Models;
    using Models.Authentication;

    [Cmdlet(VerbsCommon.New, "PartnerAzureSubscription")]
    [OutputType(typeof(SubscriptionCreationResult))]
    public class NewPartnerAzureSubscription : PartnerAsyncCmdlet
    {
        /// <summary>
        /// Gets or sets the name for the billing account.
        /// </summary>
        [Parameter(HelpMessage = "The name for the billing account.", Mandatory = true)]
        public string BillingAccountName { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the customer.
        /// </summary>
        [Parameter(HelpMessage = "The identifier for the customer.", Mandatory = true)]
        [ValidatePattern(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", Options = RegexOptions.Compiled | RegexOptions.IgnoreCase)]
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the display name for the subscription.
        /// </summary>
        [Parameter(HelpMessage = "The display for the subscription.", Mandatory = true)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or set the identifier for the indirect reseller.
        /// </summary>
        [Parameter(HelpMessage = "The identifier for the indirect reseller.", Mandatory = false)]
        public string ResellerId { get; set; }

        /// <summary>
        /// Executes the operations associated with the cmdlet.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            Scheduler.RunTask(async () =>
            {
                ISubscriptionClient client = await PartnerSession.Instance.ClientFactory.CreateServiceClientAsync<SubscriptionClient>(new[] { $"{PartnerSession.Instance.Context.Environment.AzureEndpoint}/user_impersonation" }).ConfigureAwait(false);
                ModernCspSubscriptionCreationParameters parameters = new ModernCspSubscriptionCreationParameters
                {
                    DisplayName = DisplayName,
                    ResellerId = ResellerId ?? null,
                    SkuId = "0001"
                };

                WriteObject(await client.SubscriptionFactory.CreateCspSubscriptionAsync(BillingAccountName, CustomerId, parameters, CancellationToken).ConfigureAwait(false));
            }, true);
        }
    }
}