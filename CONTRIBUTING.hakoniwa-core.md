# CONTRIBUTING.hakoniwa-core

## 目的
- hakoniwa.dllをローカル環境で作る

## 用意するもの
- VisualStudio2019（Macでもいける）

## hakoniwa.dllとデバッグ
- Unityのスクリプトからは`hakoniwa.dll`というカタチで触る。
- ログファイルは`.unitypackage`ルートに`hakoniwa_core.log`というカタチで吐き出されるので、何かあったらまずは見ておこう。

## 環境構築
1. [hakoniwa-core](https://github.com/toppers/hakoniwa-core)をクローン
2. impl/asset/server/csharp/HakoniwaCoreが基点。このディレクトリの`Hakoniwa.csproj`をダブルクリックするとVisualStudio2019のプロジェクトとして開くことが出来る。
   - JSONライブラリとか依存ライブラリの読み込みも自動でやってくれる。

## ビルド
1. メニューから「ビルド」→「Hakoniwaのビルド」をクリックする。
2. 成果物は`impl/asset/server/csharp/HakoniwaCore/bin/Debug/netstandard2.0/Hakoniwa.dll`として吐き出される。

## Unityにインポートする
1. Unityエディタで、`Assets/Plugin/Hakoniwa`を消す。
2. ビルドで作ったHakoniwa.dllを再配置する。
3. 注意しないといけないこと。
   1. Unityのバージョンによっては、`Newtonsoft_JSON`を内部に持ってて、依存関係でエラーが出ることがある。その場合は上記の.dllを削除する。