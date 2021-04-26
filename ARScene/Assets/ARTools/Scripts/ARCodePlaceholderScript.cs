using UnityEngine;

namespace ARCodeScripts
{

public class ARCodePlaceholderScript : MonoBehaviour
{
    public string[] args {get; private set; } = new string[2]{"earth", "10,0"};

    // public ARCodePlaceholder(string[] input_args) => args = input_args;
    public void SetArgs(string[] input_args) =>  args = input_args;

    void Start()
    {
        // SetArgs(new string[2]{"earth", "10,0"});
    }
}

}