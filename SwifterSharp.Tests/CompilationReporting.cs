namespace SwifterSharp.Tests
{
    public enum CompilationReporting
    {
        /// <summary>Ignores all errors and warnings</summary>
        IgnoreErrors = -1,

        /// <summary>Fails on all errors, ignores all warnings</summary>
        FailOnErrors = 0,

        /// <summary>Fails on all errors and level 1 warnings, ignores all other warnings</summary>
        FailOnErrorsAndLevel1Warnings = 1,

        /// <summary>Fails on all errors and level 1 and 2 warnings, ignores all other warnings</summary>
        FailOnErrorsAndLevel2Warnings = 2,

        /// <summary>Fails on all errors and level 1 through 3 warnings, ignores all other warnings</summary>
        FailOnErrorsAndLevel3Warnings = 3,

        /// <summary>Fails on all errors and warnings</summary>
        FailOnErrorsAndLevel4Warnings = 4,
    }
}
