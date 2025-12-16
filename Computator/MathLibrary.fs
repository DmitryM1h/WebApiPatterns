namespace Computator

module Say =
    let hello name =
        printfn "Hello %s" name

module Math =
    let add a b = a + b
    
    let subtract a b = a - b
    
    let multiply a b = a * b
    
    let divide a b = a / b
    
    let safeAdd a b =
        try
            Checked.(+) a b
        with
        | :? System.OverflowException -> 
            printfn "Ошибка переполнения!"
            0
