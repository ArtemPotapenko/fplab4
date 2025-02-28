namespace PingPong
open System.Windows.Forms
open System.Drawing

type Paddle =
    { Position: Point
      Width: int
      Height: int }

type Ball =
    { Position: Point
      Speed: Point
      Radius: int }

type GameState =
    { LeftPaddle: Paddle
      RightPaddle: Paddle
      Ball: Ball
      LeftScore: int
      RightScore: int }
module Lib =
    let initialGameState =
        { LeftPaddle =
            { Position = Point(20, 250)
              Width = 20
              Height = 100 }
          RightPaddle =
            { Position = Point(680, 250)
              Width = 20
              Height = 100 }
          Ball =
            { Position = Point(400, 250)
              Speed = Point(5, 5)
              Radius = 10 }
          LeftScore = 0
          RightScore = 0 }

    let updateBall ball =
        { ball with
            Position = Point(ball.Position.X + ball.Speed.X, ball.Position.Y + ball.Speed.Y) }

    let resetBall () =
        { Position = Point(400, 250)
          Speed = Point(5, 5)
          Radius = 10 }

    let updatePaddle (position: Point) direction height =
        let newY = position.Y + direction

        match newY with
        | y when y < 0 -> { Position = Point(position.X, 0); Width = 20; Height = height }
        | y when y + height > 600 -> { Position = Point(position.X, 600 - height); Width = 20; Height = height }
        | _ ->{ Position = Point(position.X, newY); Width = 20; Height = height }

    let checkPaddleCollision ball (paddle: Paddle) =
        let withinXBounds =
            ball.Position.X - ball.Radius < paddle.Position.X + paddle.Width
            && ball.Position.X + ball.Radius > paddle.Position.X

        let withinYBounds =
            ball.Position.Y - ball.Radius < paddle.Position.Y + paddle.Height
            && ball.Position.Y + ball.Radius > paddle.Position.Y

        withinXBounds && withinYBounds


    let checkWallCollision ball =
        ball.Position.Y - ball.Radius < 0 || ball.Position.Y + ball.Radius > 600

    let updateGameState state leftPaddleDirection rightPaddleDirection =
        let newLeftPaddle =
            updatePaddle state.LeftPaddle.Position leftPaddleDirection state.LeftPaddle.Height

        let newRightPaddle =
            updatePaddle state.RightPaddle.Position rightPaddleDirection state.RightPaddle.Height

        let newBall = updateBall state.Ball


        let newBall =
            match checkPaddleCollision newBall with
            | f when f state.LeftPaddle ->
                { newBall with
                    Speed = Point(abs newBall.Speed.X, newBall.Speed.Y) }
            | f when f state.RightPaddle ->
                { newBall with
                    Speed = Point(-abs newBall.Speed.X, newBall.Speed.Y) }
            | _ -> newBall

        let newBall =
            if
                newBall.Position.Y - newBall.Radius < 0
                || newBall.Position.Y + newBall.Radius > 600
            then
                { newBall with
                    Speed = Point(newBall.Speed.X, -newBall.Speed.Y) }
            else
                newBall

        let newBall, newLeftScore, newRightScore =
            match newBall.Position.X with
            | x when x < 0 -> (resetBall (), state.LeftScore, state.RightScore + 1)
            | x when x > 700 -> (resetBall (), state.LeftScore + 1, state.RightScore)
            | _ -> newBall, state.LeftScore, state.RightScore

        { LeftPaddle = newLeftPaddle
          RightPaddle = newRightPaddle
          Ball = newBall
          LeftScore = newLeftScore
          RightScore = newRightScore }

    let gameForm () =
        let form =
            new Form(Width = 800, Height = 600, Text = "Ping Pong Game", KeyPreview = true)


        let gameStateRef = ref initialGameState

        let moveLeftPaddleUp () =
            gameStateRef := updateGameState !gameStateRef (-10) 0

        let moveLeftPaddleDown () =
            gameStateRef := updateGameState !gameStateRef 10 0

        let moveRightPaddleUp () =
            gameStateRef := updateGameState !gameStateRef 0 (-10)

        let moveRightPaddleDown () =
            gameStateRef := updateGameState !gameStateRef 0 10

        form.KeyDown.Add(fun e ->
            match e.KeyCode with
            | Keys.W -> moveLeftPaddleUp ()
            | Keys.S -> moveLeftPaddleDown ()
            | Keys.Up -> moveRightPaddleUp ()
            | Keys.Down -> moveRightPaddleDown ()
            | _ -> ())

        form.Paint.Add(fun e ->
            e.Graphics.FillRectangle(Brushes.LightGray, 0.f, 0.f, 800.f, 600.f)

            e.Graphics.FillRectangle(
                Brushes.Green,
                (!gameStateRef).LeftPaddle.Position.X,
                (!gameStateRef).LeftPaddle.Position.Y,
                (!gameStateRef).LeftPaddle.Width,
                (!gameStateRef).LeftPaddle.Height
            )

            e.Graphics.FillRectangle(
                Brushes.Green,
                (!gameStateRef).RightPaddle.Position.X,
                (!gameStateRef).RightPaddle.Position.Y,
                (!gameStateRef).RightPaddle.Width,
                (!gameStateRef).RightPaddle.Height
            )

            e.Graphics.FillEllipse(
                Brushes.Red,
                (!gameStateRef).Ball.Position.X - (!gameStateRef).Ball.Radius,
                (!gameStateRef).Ball.Position.Y - (!gameStateRef).Ball.Radius,
                (!gameStateRef).Ball.Radius * 2,
                (!gameStateRef).Ball.Radius * 2
            )

            let font = new Font("Arial", 20F)

            e.Graphics.DrawString(
                "Score: " + string (!gameStateRef).LeftScore + " - "+ string (!gameStateRef).RightScore, font, Brushes.Black, 300.f, 20.f
            ))

        let timer = new Timer(Interval = 30)

        timer.Tick.Add(fun _ ->
            gameStateRef := updateGameState !gameStateRef 0 0
            form.Invalidate())

        timer.Start()

        form
