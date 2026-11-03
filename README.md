# Latent Voice: Fire sample

A Unity project: say ファイア and a campfire lights, say ストップ and it goes out.

This repository has the sample only. The SDK comes from the developer portal, so it won't run until you add it.

## Try it

1. Clone this repository, and open the folder in Unity 6000.0 or later (Unity Hub > Add). The campfire scene opens by itself.
2. Sign in to the developer portal, download the SDK, and create an AccessKey for **サンプルプロジェクト(焚き火)**.
3. Put the `.tgz` in this project's `Packages/` folder, then use **Window > Package Manager > + > Add package from tarball**. The Latent Voice object is added to the scene when the SDK loads.
4. Open **Tools > Latent Voice > Sync published WordSet**, paste the key, press **セットアップ**, and save the scene.
5. Press Play and say ファイア, then ストップ.

## The code

`Assets/Fire/FireOnWord.cs` is all a game needs (23 lines). This is its core: subscribe to the word event, then compare the
word that arrives.

```csharp
public string fireWord = "ファイア";
public string stopWord = "ストップ";

void OnEnable()  { Voice.OnKeywordDetected += OnWord; }
void OnDisable() { Voice.OnKeywordDetected -= OnWord; }

void OnWord(string word)
{
    if (word == fireWord) effects.Light();
    else if (word == stopWord) effects.PutOut();
}
```

`FireEffects.cs` is how this fire looks and sounds, and `FireSampleGuide.cs` is the on-screen help.

## License

This sample is under the [MIT License](LICENSE). The SDK isn't part of this repository and has its own license.

The MIT License doesn't cover the sound. `Assets/Fire/Audio/FireLoop.wav` is
[Steady Burning Fireplace Crackling 31](https://sfxmint.com/sounds/ambience-fireplace-31) from SFXMint, under CC0 1.0, and it's AI-generated.

---

# Latent Voice: Fire サンプル

「ファイア」と言うと焚き火がつき、「ストップ」と言うと消える Unity プロジェクトです。

このリポジトリにはサンプルだけが入っています。SDK は開発者ポータルから入手するので、追加するまでは動きません。

## 試す

1. このリポジトリをクローンし、Unity 6000.0 以降でフォルダーを開きます（Unity Hub > Add）。焚き火のシーンが自動で開きます。
2. 開発者ポータルにログインし、SDK をダウンロードして、**サンプルプロジェクト(焚き火)** の AccessKey を作ります。
3. `.tgz` をこのプロジェクトの `Packages/` に置き、**Window > Package Manager > + > Add package from tarball** で追加します。SDK が読み込まれると、シーンに Latent Voice のオブジェクトが追加されます。
4. **Tools > Latent Voice > Sync published WordSet** を開き、キーを貼り付けて **セットアップ** を押し、シーンを保存します。
5. Play を押して、ファイア、ストップ と話します。

## コード

`Assets/Fire/FireOnWord.cs` が、ゲームに要るコードのすべてです（23 行）。中心はこの部分で、単語のイベントを受け取り、届いた単語を比べます。

```csharp
public string fireWord = "ファイア";
public string stopWord = "ストップ";

void OnEnable()  { Voice.OnKeywordDetected += OnWord; }
void OnDisable() { Voice.OnKeywordDetected -= OnWord; }

void OnWord(string word)
{
    if (word == fireWord) effects.Light();
    else if (word == stopWord) effects.PutOut();
}
```

`FireEffects.cs` は火の見た目と音、`FireSampleGuide.cs` は画面の案内です。

## ライセンス

このサンプルは [MIT ライセンス](LICENSE)（[参考訳](LICENSE.ja.md)）です。SDK はこのリポジトリに含まれず、別のライセンスです。

音は MIT ライセンスの対象外です。`Assets/Fire/Audio/FireLoop.wav` は SFXMint の
[Steady Burning Fireplace Crackling 31](https://sfxmint.com/sounds/ambience-fireplace-31) で、CC0 1.0 の AI 生成音です。
