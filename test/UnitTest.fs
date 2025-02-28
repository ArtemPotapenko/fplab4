namespace PingPong

open NUnit.Framework
open PingPong.Lib
open System.Drawing

[<TestFixture>]
module GameTests =
    
    [<Test>]
    let ``Test updateBall`` () =
        let initialBall = { Position = Point(0, 0); Speed = Point(5, 5); Radius = 10 }
        let updatedBall = updateBall initialBall

        Assert.AreEqual(updatedBall.Position.X, 5)
        Assert.AreEqual(updatedBall.Position.Y, 5)

    [<Test>]
    let ``Test ball collision with left paddle`` () =
        let ball = { Position = Point(30, 250); Speed = Point(5, 5); Radius = 10 }
        let paddle = { Position = Point(20, 250); Width = 20; Height = 100 }
        let collision = checkPaddleCollision ball paddle

        Assert.IsTrue(collision)


    [<Test>]
    let ``Test ball collision with right paddle`` () =
        let ball = { Position = Point(670, 250); Speed = Point(-5, 5); Radius = 10 }
        let paddle = { Position = Point(680, 250); Width = 20; Height = 100 }
        let collision = checkPaddleCollision ball paddle

        Assert.IsFalse(collision)

    [<Test>]
    let ``Test updateLeftPaddle position`` () =
        let initialPaddle = { Position = Point(20, 250); Width = 20; Height = 100 }
        let newPaddle = updatePaddle initialPaddle.Position 10 initialPaddle.Height
        Assert.AreEqual(newPaddle.Position.Y, 260)

    [<Test>]
    let ``Test score update after ball passes left`` () =
        let initialState = initialGameState
        let updatedState = { initialState with Ball = { Position = Point(-10, 250); Speed = Point(5, 5); Radius = 10 } }
        let newState = updateGameState updatedState 0 0
        Assert.AreEqual(newState.LeftScore, 0)
        Assert.AreEqual(newState.RightScore, 1)

    [<Test>]
    let ``Test score update after ball passes right`` () =
        let initialState = initialGameState
        let updatedState = { initialState with Ball = { Position = Point(710, 250); Speed = Point(-5, 5); Radius = 10 } }
        let newState = updateGameState updatedState 0 0
        Assert.AreEqual(newState.LeftScore, 1)
        Assert.AreEqual(newState.RightScore, 0)
