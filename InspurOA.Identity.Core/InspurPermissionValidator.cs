using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspurOA.Identity.Core
{
    public class InspurPermissionValidator<TPermission, TKey> : IIdentityValidator<TPermission>
        where TPermission : class, IInspurPermission<TKey>
        where TKey : IEquatable<TKey>
    {

        public InspurPermissionValidator(InspurPermissionManager<TPermission, TKey> manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }

            RequireUniquePermissionCode = true;
            Manager = manager;
        }

        public bool RequireUniquePermissionCode { get; set; }

        private InspurPermissionManager<TPermission, TKey> Manager { get; set; }

        public virtual async Task<IdentityResult> ValidateAsync(TPermission item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var errors = new List<string>();
            await ValidatePermissionCode(item, errors);
            if (errors.Count > 0)
            {
                return IdentityResult.Failed(errors.ToArray());
            }

            return IdentityResult.Success;
        }


        private async Task ValidatePermissionCode(TPermission permission, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(permission.PermissionCode))
            {
                errors.Add(String.Format(CultureInfo.CurrentCulture, InspurResources.PropertyTooShort, "PermissionCode"));
            }

            if (RequireUniquePermissionCode)
            {
                var item1 = await Manager.FindByIdAsync(permission.PermissionId);
                var item2 = await Manager.FindByCodeAsync(permission.PermissionCode);
                if (item1 == null)   //Create
                {
                    if (item2 != null)
                    {
                        errors.Add(String.Format(CultureInfo.CurrentCulture, InspurResources.DuplicateCode, permission.PermissionCode));
                    }
                }
                else                 //Update
                {
                    if (item2 != null && EqualityComparer<TKey>.Default.Equals(item2.PermissionId, permission.PermissionId))
                    {
                        errors.Add(String.Format(CultureInfo.CurrentCulture, InspurResources.DuplicateCode, permission.PermissionCode));
                    }
                }               
            }
        }
    }
}
