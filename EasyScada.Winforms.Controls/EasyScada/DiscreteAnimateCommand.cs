using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyScada.Winforms.Connector;

namespace EasyScada.Winforms.Controls
{
    //public class DiscreteAnimateCommand : AnimateCommandBase
    //{
    //    public override string PathToHighTag
    //    {
    //        get { return base.PathToHighTag; }
    //        set
    //        {
    //            base.PathToHighTag = value;
    //            PathToLowTag = value;
    //        }
    //    }
    //    public override string PathToLowTag
    //    {
    //        get { return base.PathToLowTag; }
    //        set
    //        {
    //            base.PathToLowTag = value;
    //            PathToHighTag = value;
    //        }
    //    }

    //    private CompareMode _compareMode = CompareMode.Equal;
    //    public override CompareMode CompareMode
    //    {
    //        get { return _compareMode; }
    //        set
    //        {
    //            if (value == CompareMode.Equal || value == CompareMode.NotEqual)
    //                _compareMode = value;
    //        }
    //    }

    //    public DiscreteAnimateCommand() : base()
    //    {
    //        AnimateMode = AnimateMode.Discrete;
    //        AnimatePiority = AnimatePiority.Medium;
    //        CompareValueMode = CompareValueMode.Const;
    //    }

    //    public override void Execute()
    //    {
    //        try
    //        {
    //            if (Connector != null && BaseControl != null && PropertyName != null && LinkedTag != null)
    //            {
    //                if (TryGetCompareValue(out decimal compareValue) && decimal.TryParse(LinkedTag.Value, out decimal currentValue))
    //                {
    //                    bool sucess = false;
    //                    if (CompareMode == CompareMode.Equal)
    //                        sucess = currentValue == compareValue;
    //                    else
    //                        sucess = currentValue != compareValue;

    //                    if (sucess)
    //                        SetValue(BaseControl, GetAnimateValue(), PropertyName);
    //                }
    //            }
    //        }
    //        catch { }
    //    }
    //}
}
