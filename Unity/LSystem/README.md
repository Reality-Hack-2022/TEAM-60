L-Systems in Unity
================

L-System is a rewriting system used mainly to model the development of plants. L-Systems in Unity enables you to rapidly create plants in your Unity games.


----------

### Getting Started

Add LSystemExecutor to a game object in your scene. Set a L-System definition (e.g., "Assets/Defs/3D Tree 1.txt") and rendering properties. Start the game.

		// 3D Tree 1.txt

		axiom=F
		angle=22.5
		number of derivations=3

		F=(1)F[-&^F][^++&F]||F[--&^F][+&F]


![LSystemExecutor in object inspector](http://www.pedroboechat.com/images/LSystemInUnity1.png)

![3D tree](http://www.pedroboechat.com/images/LSystemInUnity2.png)

----------

### Advanced Usage

L-System processing is divided into three phases: parsing, derivation and interpretation. L-System on Unity's API map each phase to a static class.


----------

#### Parsing

Method:

> LSystemParser.Parse()

Input: 

 - l-system definition [string]

Output: 

 - axiom [string]
 - angle [float]
 - derivations [int]
 - productions [Dictionary&lt;string, List&lt;Production&gt;&gt;]

Example:


		string axiom;
		float angle;
		int derivations;
		Dictionary<string, List<Production>> productions;
		LSystemParser.Parse(
				file.text,
				out axiom,
				out angle,
				out derivations,
				out productions);


----------

#### Derivation

Method:

> LSystemDeriver.Derive()

Input:

 - axiom [string]
 - angle [float]
 - derivations [int]
 - productions [Dictionary&lt;string, List&lt;Production&gt;&gt;]

Output:

 - moduleString [string]

Example:

		string moduleString;
		LSystemDeriver.Derive(
				axiom,
				angle,
				derivations,
				rules,
				out moduleString);


----------

#### Interpretation

Method:

> LSystemInterpreter.Interpret()

Input:

 - num. segment axial samplers [int]
 - num. segment radial samplers [int]
 - segment width [float]
 - segment height [float]
 - leaf size [float]
 - leaf axial density [int]
 - leaf radial density [int]
 - use foliage [bool]
 - narrow branches [bool]
 - leaf material [UnityEngine.Material]
 - trunk material [UnityEngine.Material]
 - angle [float]
 - moduleString [string]

Output:

 - leaves [UnityEngine.GameObject]
 - trunk [UnityEngine.GameObject]

Example:


		GameObject leaves, trunk;
		LSystemInterpreter.Interpret(
				segmentAxialSamples,
				segmentRadialSamples,
				segmentWidth,
				segmentHeight,
				leafSize,
				leafAxialDensity,
				leafRadialDensity,
				useFoliage,
				narrowBranches,
				leafMaterial,
				trunkMaterial,
				angle,
				moduleString,
				out leaves,
				out trunk);
            
  
