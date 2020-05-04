namespace UITest.Tests

open NUnit.Framework
open Xamarin.UITest

[<TestFixture>]
module ``Tests`` =
    let mutable app : IApp = null
    
    [<SetUp>]
    let ``Setup`` () =
        app <- ConfigureApp.Android.InstalledApp("com.codingsanta.fakenuketryout").EnableLocalScreenshots().StartApp() :> IApp
        
    [<Test>]
    let ``Mock test`` () =
        Assert.Pass()