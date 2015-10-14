namespace Perspex
{
    //
    // Summary:
    //     Describes the breaking condition around an inline object.
    public enum LineBreakCondition
    {
        //
        // Summary:
        //     Break if not prohibited by another object.
        BreakDesired = 0,
        //
        // Summary:
        //     Break if allowed by another object.
        BreakPossible = 1,
        //
        // Summary:
        //     Break always prohibited unless the other object is set to System.Windows.LineBreakCondition.BreakAlways.
        BreakRestrained = 2,
        //
        // Summary:
        //     Break is always allowed.
        BreakAlways = 3
    }
}
