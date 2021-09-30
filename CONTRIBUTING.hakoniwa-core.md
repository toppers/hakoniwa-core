# CONTRIBUTING.hakoniwa-core

## Hakoniwa.dllをローカル環境で作る

2パターンのやり方を用意しました。

1. パターン①VisualStudio2019を使う
2. パターン②Dockerを使う

### パターン①VisualStudio2019を使う

#### 用意するもの

- VisualStudio2019（Macでもいける）

#### 環境構築

1. [hakoniwa-core](https://github.com/toppers/hakoniwa-core)をクローン

   ```bash
   git clone https://github.com/toppers/hakoniwa-core.git
   ```

2. impl/asset/server/csharp/HakoniwaCoreが基点。このディレクトリの`Hakoniwa.csproj`をダブルクリックするとVisualStudio2019のプロジェクトとして開くことが出来る。
   - JSONライブラリとか依存ライブラリの読み込みも自動でやってくれる。

### ビルド

1. メニューから「ビルド」→「Hakoniwaのビルド」をクリックする。
2. 成果物は`impl/asset/server/csharp/HakoniwaCore/bin/Debug/netstandard2.0/Hakoniwa.dll`として吐き出される。

### パターン②Dockerを使う

#### 用意するもの(Docker)

- Docker（v20.10.8で動作確認）

#### 環境構築(Docker)

1. [hakoniwa-core](https://github.com/toppers/hakoniwa-core)をクローン

   ```bash
   git clone https://github.com/toppers/hakoniwa-core.git
   ```

#### ビルド(Docker)

1. リポジトリのトップに移動

   ```bash
   cd hakoniwa-core
   ```

2. ビルドする

   ```bash
   bash ./impl/asset/server/csharp/HakoniwaCore/build_by_docker.bash Rebuild Debug
   # "Usage: $0 {Rebuild|Build} {Release|Debug}"
   ```

3. エラーが無ければ`dst`ディレクトリに`Hakoniwa.dll`が作成されます。

---

## Hakoniwa.dllをUnityで使う

### インポートする

1. Unityエディタで、`Assets/Plugin/Hakoniwa`を消す。
2. ビルドで作ったHakoniwa.dllを再配置する。
3. 注意しないといけないこと。
   1. Unityのバージョンによっては、`Newtonsoft_JSON`を内部に持ってて、依存関係でエラーが出ることがある。その場合は上記の.dllを削除する。

### hakoniwa.dllとデバッグ

- Unityのスクリプトからは`hakoniwa.dll`というカタチで触る。
- ログファイルは`.unitypackage`ルートに`hakoniwa_core.log`というカタチで吐き出されるので、何かあったらまずは見ておこう。
