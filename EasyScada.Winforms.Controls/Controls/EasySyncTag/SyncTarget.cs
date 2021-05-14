using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    public class SyncTarget
    {

        [Editor(typeof(PathToTagPropertyEditor), typeof(UITypeEditor))]
        [Description("Select path to tag for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string SourceTagPath { get; set; }

        [Editor(typeof(PathToTagPropertyEditor), typeof(UITypeEditor))]
        [Description("Select path to tag for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string TargetTagPath { get; set; }

        [Description("Select driver connector for control")]
        [Browsable(false), Category(DesignerCategory.EASYSCADA)]
        public IEasyDriverConnector Connector => EasyDriverConnectorProvider.GetEasyDriverConnector();

        ITag _targetTag;
        [Browsable(false)]
        public ITag TargetTag
        {
            get
            {
                if (_targetTag == null)
                {
                    _targetTag = Connector?.GetTag(TargetTagPath);
                }
                else
                {
                    if (_targetTag.Path != TargetTagPath)
                        _targetTag = Connector?.GetTag(TargetTagPath);
                }
                return _targetTag;
            }
        }

        ITag _sourceTag;
        [Browsable(false)]
        public ITag SourceTag
        {
            get
            {
                if (_sourceTag == null)
                {
                    _sourceTag = Connector?.GetTag(SourceTagPath);
                }
                else
                {
                    if (_sourceTag.Path != SourceTagPath)
                        _sourceTag = Connector?.GetTag(SourceTagPath);
                }
                return _sourceTag;
            }
        }

        public override string ToString()
        {
            return TargetTagPath;
        }
    }
}
