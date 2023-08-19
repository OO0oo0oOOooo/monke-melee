public interface IDMSlider
{
    // Find a better way to pass information to the command.
    void EnterCommand(string str);
    void RightCommand(float value, string str);
    void LeftCommand(float value, string str);
}
