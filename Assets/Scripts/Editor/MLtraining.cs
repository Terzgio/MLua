using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class MLtraining : EditorWindow
{
    private Vector2 scrollPosition;

    string userName = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    string path;
    string batPath;
    string yamlPath;
    string yamlfilepath;

    bool resumeTraining = false;

    bool showTrainparam = false;
    bool showSacparam = false;
    bool showPpoparam = false;

    int sel_mlmethod = 0;
    int sel_learnrateschedule = 0;
    int sel_viscodetype = 0;

    string strCmdText;
    string behaviorName = "Collector";
    string testName = "test_01";
    string content;
    string[] trainertype = new string[]
    {
        "ppo", "sac"
    };


    //hyperparameters
    //general
    float batchsize = 128f;
    float buffersize = 2048f;
    float learningrate = 0.0003f;
    string[] learningRateSchedule = new string[]
    {
        "linear", "constant"
    };

    //network settings
    bool normalize = false;
    int hiddenunits = 256;
    int numlayers = 2;
    string[] visencodetype = new string[]
    {
        "simple", "nature_cnn", "resnet", "match3", "fully_connected"
    };

    //reward signals
    float gamma = 0.99f;
    float strength = 1.0f;

    //other
    int keepcheckpoints = 5;
    int maxsteps = 100000;
    int timehorizon = 128;
    int summaryfreq = 5000;
    bool threaded = true;

    //ppo only
    float beta = 0.01f;
    float epsilon = 0.2f;
    float lambd = 0.95f;
    int numepoch = 3;

    //sac only
    int bufferinitsteps = 0;
    float tau = 0.005f;
    int stepsperupdate = 10;
    bool savereplaybuffer = false;
    float initcoef = 0.5f;
    
    
    [MenuItem("Window/ML Training")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<MLtraining>("Ml Training");
        
    }

    private void OnGUI()
    {
        path = userName + "\\anaconda3\\Scripts";
        batPath = userName + "\\anaconda3\\Scripts\\customactivate.bat";
        yamlPath = userName + "\\Documents\\Code";

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);


        GUILayout.Label("Machine Learning Training");

        GUILayout.Label("Anaconda Path:\n" + path + "\n\n");

        behaviorName = EditorGUILayout.TextField("Behavior name:", behaviorName);

        testName = EditorGUILayout.TextField("Test name:", testName);
        sel_mlmethod = EditorGUILayout.Popup("ML method:", sel_mlmethod, trainertype);
        resumeTraining = EditorGUILayout.Toggle("Resume Training", resumeTraining);


        GUILayout.Label("\n");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create bat file"))
        {

            baseBat();
            //Extra custom commands for bat file

            content = "\n\nCALL conda.bat activate ml-agents";
            System.IO.File.AppendAllText(batPath, content);

            if (resumeTraining)
            {
                content = "\nmlagents-learn \"C:\\Users\\George\\Documents\\Code\\" + behaviorName + ".yaml\" --run-id " + testName + " --resume";
            }
            else
            {
                content = "\nmlagents-learn \"C:\\Users\\George\\Documents\\Code\\" + behaviorName + ".yaml\" --run-id " + testName;
            }

            System.IO.File.AppendAllText(batPath, content);


        }


        if (GUILayout.Button("Create yaml file"))
        {
            yamlfilepath = yamlPath + "\\" + behaviorName + ".yaml";
            yamlstart();
            if (sel_mlmethod == 0)
            {
                
                createyamlppo();
            }
            else if (sel_mlmethod == 1)
            {
                createyamlsac();
            }
            yamlend();

        }
        EditorGUILayout.EndHorizontal();

        showTrainparam = EditorGUILayout.Foldout(showTrainparam, "Training parameters");
        if (showTrainparam)
        {
            GUILayout.Label("Hyperparameters:");
            batchsize = EditorGUILayout.FloatField("Batch size:", batchsize);
            buffersize = EditorGUILayout.FloatField("Buffer size:", buffersize);
            learningrate = EditorGUILayout.FloatField("Learning rate:", learningrate);
            sel_learnrateschedule = EditorGUILayout.Popup("Learnig rate schedule:", sel_learnrateschedule, learningRateSchedule);
            GUILayout.Label("Network Settings:");
            normalize = EditorGUILayout.Toggle("Normalize", normalize);
            hiddenunits = EditorGUILayout.IntField("Hidden units:", hiddenunits);
            numlayers = EditorGUILayout.IntField("Number of layers:", numlayers);
            sel_viscodetype = EditorGUILayout.Popup("Vis encode type:", sel_viscodetype, visencodetype);
            GUILayout.Label("Reward signals:");
            gamma = EditorGUILayout.FloatField("Gamma:", gamma);
            strength = EditorGUILayout.FloatField("Strength:", strength);
            GUILayout.Label("Other:");
            keepcheckpoints = EditorGUILayout.IntField("Keep checkpoints:", keepcheckpoints);
            maxsteps = EditorGUILayout.IntField("Max steps:", maxsteps);
            timehorizon = EditorGUILayout.IntField("Time horizon:", timehorizon);
            summaryfreq = EditorGUILayout.IntField("Summary frequency:", summaryfreq);
            threaded = EditorGUILayout.Toggle("Threaded", threaded);

        }
        GUILayout.Label("\n");

        showPpoparam = EditorGUILayout.Foldout(showPpoparam, "PPO only parameters");
        if (showPpoparam)
        {
            beta = EditorGUILayout.FloatField("Beta:", beta);
            epsilon = EditorGUILayout.FloatField("Epsilon:", epsilon);
            lambd = EditorGUILayout.FloatField("Lambda:", lambd);
            numepoch = EditorGUILayout.IntField("Number of epochs:", numepoch);
        }


        showSacparam = EditorGUILayout.Foldout(showSacparam, "SAC only parameters");
        if (showSacparam)
        {
            bufferinitsteps = EditorGUILayout.IntField("Initial buffer steps:", bufferinitsteps);
            tau = EditorGUILayout.FloatField("Tau:", tau);
            stepsperupdate = EditorGUILayout.IntField("Steps per update:", stepsperupdate);
            savereplaybuffer = EditorGUILayout.Toggle("Save replay buffer", savereplaybuffer);
            initcoef = EditorGUILayout.FloatField("Initial entropy coefficient :", initcoef);
        }



        GUILayout.Label("\n");
        if (GUILayout.Button("Start Training!"))
        {
            strCmdText = "/K cd " + path + " & customactivate.bat";
            System.Diagnostics.Process.Start("CMD.exe", strCmdText);

        }

        EditorGUILayout.EndScrollView();


    }
    void baseBat()
    {
        //Basic bat file
        System.IO.File.WriteAllText(batPath, "@set \"_args1=%1\"");

        content = "\n@set _args1_first=%_args1:~0,1%";
        System.IO.File.AppendAllText(batPath, content);

        content = "\n@set _args1_last=%_args1:~-1%";
        System.IO.File.AppendAllText(batPath, content);

        content = "\n@set _args1_first=%_args1_first:\"=+%";
        System.IO.File.AppendAllText(batPath, content);

        content = "\n@set _args1_last=%_args1_last:\"=+%";
        System.IO.File.AppendAllText(batPath, content);

        content = "\n@set _args1=";
        System.IO.File.AppendAllText(batPath, content);

        content = "\n@set _args1=\n";
        System.IO.File.AppendAllText(batPath, content);

        content = "\n@if \"%_args1_first%\"==\"+\" if NOT \"%_args1_last%\"==\"+\" (";
        System.IO.File.AppendAllText(batPath, content);

        content = "\n    @CALL \"%~dp0..\\condabin\\conda.bat\" activate";
        System.IO.File.AppendAllText(batPath, content);

        content = "\n    @GOTO :End\n)";
        System.IO.File.AppendAllText(batPath, content);

        content = "\n@CALL \"%~dp0..\\condabin\\conda.bat\" activate %*";
        System.IO.File.AppendAllText(batPath, content);

        content = "\n\n:End";
        System.IO.File.AppendAllText(batPath, content);

        content = "\n@set _args1_first=\n@set _args1_last=";
        System.IO.File.AppendAllText(batPath, content);
    }

    void createyamlppo()
    {
        content = "\n      beta: " + beta;
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n      epsilon: " + epsilon;
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n      lambd: " + lambd;
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n      num_epoch: " + numepoch;
        System.IO.File.AppendAllText(yamlfilepath, content);
    }

    void createyamlsac()
    {
        content = "\n      buffer_init_steps: " + bufferinitsteps;
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n      tau: " + tau;
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n      steps_per_update: " + stepsperupdate;
        System.IO.File.AppendAllText(yamlfilepath, content);

        if (savereplaybuffer) { content = "\n      save_replay_buffer: true"; } else content = "\n      save_replay_buffer: false";
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n      init_entcoef: " + initcoef;
        System.IO.File.AppendAllText(yamlfilepath, content);


    }


    void yamlstart()
    {
        System.IO.File.WriteAllText(yamlfilepath, "behaviors:");

        content = "\n  " + behaviorName + ":";
        System.IO.File.AppendAllText(yamlfilepath, content);
                
        content = "\n    trainer_type: " + trainertype[sel_mlmethod];
        System.IO.File.AppendAllText(yamlfilepath, content);

        System.IO.File.AppendAllText(yamlfilepath, "\n    hyperparameters:");

        content = "\n      batch_size: " + batchsize;
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n      buffer_size: " + buffersize;
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n      learning_rate: " + learningrate;
        System.IO.File.AppendAllText(yamlfilepath, content);
    }

    void yamlend()
    {
        content = "\n      learning_rate_schedule: " + learningRateSchedule[sel_learnrateschedule];
        System.IO.File.AppendAllText(yamlfilepath, content);

        System.IO.File.AppendAllText(yamlfilepath, "\n    network_settings:");

        if (normalize) { content = "\n      normalize: true"; } else content = "\n      normalize: false";
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n      hidden_units: " + hiddenunits;
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n      num_layers: " + numlayers;
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n      vis_encode_type: " + visencodetype[sel_viscodetype];
        System.IO.File.AppendAllText(yamlfilepath, content);

        System.IO.File.AppendAllText(yamlfilepath, "\n    reward_signals:");
        System.IO.File.AppendAllText(yamlfilepath, "\n      extrinsic:");

        content = "\n        gamma: " + gamma;
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n        strength: " + strength;
        System.IO.File.AppendAllText(yamlfilepath, content);
        
        content = "\n    keep_checkpoints: " + keepcheckpoints;
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n    max_steps: " + maxsteps;
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n    time_horizon: " + timehorizon;
        System.IO.File.AppendAllText(yamlfilepath, content);

        content = "\n    summary_freq: " + summaryfreq;
        System.IO.File.AppendAllText(yamlfilepath, content);

        if (threaded) { content = "\n    threaded: true"; } else content = "\n    threaded: false";
        System.IO.File.AppendAllText(yamlfilepath, content);

    }


    public void OnInspectorUpdate()
    {
        this.Repaint();
    }
}
