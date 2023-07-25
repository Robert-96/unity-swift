import Foundation

@objc public class SwiftToUnity: NSObject {
    @objc public static let shared = SwiftToUnity()

    /// Sends a "Hello World" message to the "Canvas" GameObject and calls the "OnMessageReceived" script method on that object with the "Hello World!" message.
    @objc public func swiftSendHelloWorldMessage() {
        // The UnitySendMessage function has three parameters: the name of the target GameObject, the script method to call on that object and the message string to pass to the called method.
        UnitySendMessage("Canvas", "OnMessageReceived", "Hello World!");
    }

    /// Returns the "Hello, Swift!" string.
    ///
    /// - Returns: The "Hello, Swift!" string.
    @objc public func swiftHelloWorld() -> String {
        return "Hello, Swift!"
    }

    /// Adds two integers and returns the result.
    ///
    /// - Parameters:
    ///   - x: The first integer.
    ///   - y: The second integer.
    /// - Returns: The sum of the two integers.
    @objc public func swiftAdd(_ x: Int, _ y: Int) -> Int {
        return x + y
    }

    /// Concatenates two strings and returns the result.
    ///
    /// - Parameters:
    ///   - x: The first string.
    ///   - y: The second string.
    /// - Returns: The concatenated string.
    @objc public func swiftConcatenate(_ x: String, y: String) -> String {
        return x + y
    }
}
