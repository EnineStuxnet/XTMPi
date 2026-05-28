# eXtraTMP (XTMP)

**eXtraTMP**는 기존 폰트에 대체 폰트를 추가하거나 다른 폰트로 완전히 교체할 수 있도록 해주는 간단한 BepInEx 플러그인입니다.

## 사용 가이드

### 1. 폰트 또는 폰트 에셋 준비하기

폰트는 트루타입 또는 에셋 번들을 통해 불러올 수 있습니다.

#### 트루타입 폰트 불러오기
1. 폰트 파일(`*.ttf`)을 `BepInEx/plugins/XTMP` 폴더에 넣습니다.
2. 설정 파일에 정확한 파일명으로 폰트를 지정합니다.

#### 에셋 번들 불러오기
1. [Unity에서 TextMeshPro 에셋을 생성합니다](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.2/manual/FontAssetsCreator.html#creating-a-font-asset).
2. [AssetBundles-Browser](https://github.com/Unity-Technologies/AssetBundles-Browser)를 사용하여 에셋을 번들로 빌드합니다.
3. 번들 파일(빈 확장자 또는 `*.bundle`)을 `BepInEx/plugins/XTMP` 폴더에 넣습니다.

### 2. 설정 파일 만들기

1. 아래의 [설정 구문](#설정-구문) 섹션을 참고하여 설정 파일을 작성합니다.
2. 완성된 설정 파일(`*.ini`)을 `BepInEx/plugins/XTMP` 폴더에 넣습니다.

### 3. 디렉토리 구조

모든 설정이 완료되면 `BepInEx/plugins/XTMP` 폴더는 다음과 같은 구조를 가져야 합니다:

```text
BepInEx/plugins/XTMP/
├── bundles/
│   ├── ko-KR.bundle
│   ├── ko-KR.bundle.ini
├── fonts/
│   ├── ko-KR/
│   │   ├── D2Coding.ttf
│   │   ├── Galmuri7.ttf
│   │   └── NotoSansKR.ttf
│   └── ko-KR.bundle.ini
└── XTMP.dll
```

## 설정 구문

> **참고:** 설정 파일 형식은 INI와 비슷하지만, 실제 표준 INI 구문을 지원하지는 않습니다.

```ini
# 전역 대체
# 섹션 헤더 없이 나열된 폰트는 TMP_Settings의 전역 대체 목록에 자동으로 추가됩니다.
Galmuri7

# 직접 교체
# '->' 연산자를 사용하여 원본 폰트를 새 폰트로 완전히 교체합니다.
# 구문: [원본폰트이름 -> 교체할폰트이름]
[ExperimentIEatCrayons -> KCCKimHoon SDF]

# 특정 폰트 대체
# 원본 폰트의 이름을 섹션 헤더로 지정하면 해당 폰트에만 대체 폰트를 할당할 수 있습니다.
[Retro Gaming]
NeoDunggeunmoPro

# 한 섹션 아래에 여러 폰트를 나열하면 순서대로 대체 폰트로 적용됩니다.
[LiberationSans SDF]
NotoSansKR SDF
NotoSansJP SDF
```