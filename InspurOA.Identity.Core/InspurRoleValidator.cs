using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public class InspurRoleValidator<TRole> : InspurRoleValidator<TRole, string>
        where TRole : class, IInspurRole<string>
    {
        public InspurRoleValidator(InspurRoleManager<TRole, string> manager) : base(manager)
        {

        }
    }

    public class InspurRoleValidator<TRole, TKey> : IIdentityValidator<TRole>
        where TRole : class, IInspurRole<TKey>
        where TKey : IEquatable<TKey>
    {
        public InspurRoleValidator(InspurRoleManager<TRole, TKey> manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }

            RequireUniqueRoleCode = true;
            Manager = manager;
        }

        public bool RequireUniqueRoleCode { get; set; }

        private InspurRoleManager<TRole, TKey> Manager { get; set; }

        public virtual async Task<IdentityResult> ValidateAsync(TRole item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var errors = new List<string>();
            await ValidateRoleCode(item, errors);
            if (errors.Count > 0)
            {
                return IdentityResult.Failed(errors.ToArray());
            }

            return IdentityResult.Success;
        }

        private async Task ValidateRoleCode(TRole role, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(role.RoleCode))
            {
                errors.Add(String.Format(CultureInfo.CurrentCulture, InspurResources.PropertyTooShort, "RoleCode"));
            }
            if (RequireUniqueRoleCode)
            {
                var item1 = await Manager.FindByIdAsync(role.RoleId);
                var item2 = await Manager.FindByCodeAsync(role.RoleCode);
                if (item1 == null)   //Create
                {
                    if (item2 != null)
                    {
                        errors.Add(String.Format(CultureInfo.CurrentCulture, InspurResources.DuplicateCode, role.RoleCode));
                    }
                }
                else                //Update
                {
                    if (item2 != null && EqualityComparer<TKey>.Default.Equals(item2.RoleId, role.RoleId))
                    {
                        errors.Add(String.Format(CultureInfo.CurrentCulture, InspurResources.DuplicateCode, role.RoleCode));
                    }
                }
            }
        }
    }
}
