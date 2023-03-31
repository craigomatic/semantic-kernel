// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using Python.Runtime;

namespace PythonSkillRunner;
internal class PythonRandomActivitySkill
{

        [SKFunction("Gets a random activity from an API")]
        [SKFunctionName("GetRandomActivity")]
        public string GetRandomActivityAsync()
        {
            PythonEngine.Initialize();
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                // Env variable PYTHONPATH contains the directory for the pythonskills module
                var pythonpath = Environment.GetEnvironmentVariable("PYTHONPATH");
                sys.path.insert(0, pythonpath);
                dynamic skill = Py.Import("pythonskills.randomActivitySkill");
                return (string) skill.RandomActivitySkill.getRandomActivity();
            }
        }
}
