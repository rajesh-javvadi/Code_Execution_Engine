import java.util.Scanner;

public class SumOfTwo {
    public static void main(String[] args) {
        // Create a Scanner object for input
        Scanner scanner = new Scanner(System.in);

        // Declare variables
        int num1, num2, sum;

        // Input two numbers
        num1 = scanner.nextInt();
        num2 = scanner.nextInt();

        // Calculate the sum
        sum = num1 + num2;

        // Display the result
        System.out.println(sum);

        // Close the scanner
        scanner.close();
    }
}