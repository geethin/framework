using SharpYaml.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using GT.Agreement.Utils;
using Microsoft.CodeAnalysis.CSharp;
using GT.CLI.Common;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.IO;
using static GT.CLI.Common.TypeHelper;

namespace GT.CLI.Commands
{
    /// <summary>
    /// 表单生成
    /// </summary>
    public class NgFormGenerate : GenerateBase
    {
        public NgFormGenerate()
        {
        }

        public void Add(PropertyInfo input)
        {
        }

        /// <summary>
        /// 生成添加组件
        /// </summary>
        public string GenerateAddForm(List<PropertyInfo> propertyInfos, string name)
        {
            var formControls = "";
            foreach (var input in propertyInfos)
            {
                formControls += input.ToNgInputControl();
            }
            var tmp = @$"<legend>
  <h2>添加{name}</h2>
</legend>
<form [formGroup]=""formGroup"" (ngSubmit)=""add()"">
  <div fxLayout=""row wrap"" fxLayoutAlign=""start start"" fxLayoutGap=""8px"">
{formControls}
  </div>
  <div fxLayout=""row"" fxLayoutAlign=""start start"" fxLayoutGap=""8px"" class=""mt-2"">
    <button mat-flat-button color=""primary"" type=""submit"">添加</button>
  </div>
</form>";
            return tmp;
        }

        public string GenerateEditForm(List<PropertyInfo> propertyInfos, string name)
        {
            var formControls = "";
            foreach (var input in propertyInfos)
            {
                formControls += input.ToNgInputControl();
            }
            var tmp = @$"<legend>
  <h2>编辑{name}</h2>
</legend>
<form *ngIf=""!isLoading"" [formGroup]=""formGroup"" (ngSubmit)=""edit()"">
  <div fxLayout=""row wrap"" fxLayoutAlign=""start start"" fxLayoutGap=""8px"">
{formControls}
  </div>
  <div fxLayout=""row"" fxLayoutAlign=""start start"" fxLayoutGap=""8px"" class=""mt-2"">
    <button mat-flat-button color=""primary"" type=""submit"">更新</button>
  </div>
</form>";
            return tmp;
        }

        public string Build()
        {
            return default;
        }
    }


}
