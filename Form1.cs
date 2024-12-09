
using System.Diagnostics;

namespace AsyncForm;

public partial class Form1 : Form
{
    private Button startButton;
    private ProgressBar progressBar;
    private Label resultLabel;
    private Button cancelButton;
    private Button randomButton;
    private Label randomLabel;

    static List<String> _urlList = new List<string> {
        "https://example.com/",
        "https://microsoft.com",
        "https://learn.microsoft.com",
        "https://dotnet.microsoft.com",
        "https://github.com",
        "https://mozilla.org",
        "https://wikipedia.org",
        "https://bbc.com",
        "https://developer.apple.com",
        "https://gnu.org",
        "https://amazon.com",
        "https://facebook.com",
        "https://instagram.com",
        "https://quora.com",
        "https://netflix.com",
        "https://medium.com",
        "https://yahoo.com",
        "https://stackoverflow.blog",
        "https://wordpress.com",
        "https://khanacademy.org",

    };

    static readonly HttpClient client = new HttpClient();

    static char[] delimiters = new char[] {' ', '\r', '\n' };
    static async Task<(int,String)> FetchContent(String url){

        var html = await client.GetStringAsync(url);
        //Console.WriteLine(response);
        int words = html.Split(delimiters,StringSplitOptions.RemoveEmptyEntries).Length;

        return (words, url);
    }

    static List<Task> _requestTasks = new List<Task>();

    public Form1()
    {
        InitializeComponent();
        InitializeCustomComponents();
    }

    private void InitializeCustomComponents()
    {
       // Initialize startButton
       startButton = new Button();
       startButton.Text = "Start";
       startButton.Location = new Point(50, 30);
       startButton.Size = new Size(150, 50);
       startButton.Click += startButton_ClickAsync;

       cancelButton = new Button();
       cancelButton.Text = "Cancel";
       cancelButton.Location = new Point(200, 30);
       cancelButton.Size = new Size(150, 50);
       //cancelButton.Click += cancelButton_Click;

    //    randomButton = new Button();
    //    randomButton.Text = "Click Me!";
    //    randomButton.Location = new Point(50, 200);
    //    randomButton.Size = new Size(150,50);
    //    randomButton.Click += randomButton_Click;

    //    randomLabel = new Label();
    //    randomLabel.Text = "";
    //    randomLabel.Location = new Point(50, 250);
    //    randomLabel.Size = new Size(400, 50);

       // Initialize progressBar
       progressBar = new ProgressBar();
       progressBar.Location = new Point(50, 100);
       progressBar.Size = new Size(400, 20);

       // Initialize resultLabel
       resultLabel = new Label();
       resultLabel.Text = "";
       resultLabel.Location = new Point(50, 150);
       resultLabel.Size = new Size(800, 300);
       resultLabel.AutoSize = true;

       // Add controls to the form
       this.Controls.Add(startButton);
       this.Controls.Add(progressBar);
       this.Controls.Add(resultLabel);
       this.Controls.Add(cancelButton);
       this.Controls.Add(randomButton);
       this.Controls.Add(randomLabel);
    }

    private async void startButton_ClickAsync(object? sender, EventArgs e)
    {
        int totalWords = 0;
        resultLabel.Text = "";
        Stopwatch sw = new Stopwatch();

        sw.Start();

        for(int i = 0; i < _urlList.Count; i++){

                var resultTask = FetchContent(_urlList[i]);
                _requestTasks.Add(resultTask);
            }
        
        int totalTasks = _requestTasks.Count;

        while (_requestTasks.Count > 0){

            Task finishedTask = await Task.WhenAny(_requestTasks);
            
            if (finishedTask is Task<(int,String)> intTask) {
                (int wordCount,String url) result = await intTask;
                resultLabel.Text += result.url + ": " + result.wordCount.ToString() + " words \n" ;
                totalWords += result.wordCount;
            };
            
            _requestTasks.Remove(finishedTask);
            progressBar.Value = (totalTasks - _requestTasks.Count) * (100/totalTasks);

        }

        sw.Stop();

        resultLabel.Text += "Total Words: " + totalWords.ToString() + " in " + sw.Elapsed.ToString();
        //Console.WriteLine("Done!");

        
    }

    private void randomButton_Click(object sender, EventArgs e){

        randomLabel.Text = randomLabel.Text == "hi" ? "" : "hi";
    }
}
