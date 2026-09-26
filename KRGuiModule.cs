using Celeste;

namespace Celeste.Mod.BetterKoreanTranslation;

public sealed class BetterKoreanTranslationModule : EverestModule {
    private const string KoreanLanguageId = "korean";
    private const string FontFace = "RenogareKR";
    private const int FontSize = 62;
    private const string ExtendedVariantModNameKey = "MODNAME_EXTENDEDVARIANTMODE";
    private const string ExtendedVariantOptionsKeyPrefix = "MODOPTIONS_EXTENDEDVARIANTS";
    private const string IncorrectVariantTerm = "별형";
    private const string CorrectVariantTerm = "변형";

    public override void Load() {
        On.Celeste.Dialog.LoadLanguage += OnLoadLanguage;
        On.Celeste.BirdTutorialGui.ctor += OnBirdTutorialGuiCtor;
    }

    public override void Unload() {
        On.Celeste.Dialog.LoadLanguage -= OnLoadLanguage;
        On.Celeste.BirdTutorialGui.ctor -= OnBirdTutorialGuiCtor;
    }

    private static Language OnLoadLanguage(
        On.Celeste.Dialog.orig_LoadLanguage orig,
        string filename
    ) {
        Language language = orig(filename);

        if (language?.Id?.Equals(
                KoreanLanguageId,
                System.StringComparison.OrdinalIgnoreCase
            ) == true) {
            language.FontFace = FontFace;
            language.FontFaceSize = FontSize;
            ReplaceExtendedVariantTerms(language);
        }

        return language;
    }

    private static void ReplaceExtendedVariantTerms(Language language) {
        foreach (string key in new System.Collections.Generic.List<string>(
                     language.Dialog.Keys
                 )) {
            if (!IsExtendedVariantDialogKey(key)) {
                continue;
            }

            language.Dialog[key] = language.Dialog[key].Replace(
                IncorrectVariantTerm,
                CorrectVariantTerm,
                System.StringComparison.Ordinal
            );

            if (language.Cleaned.TryGetValue(key, out string cleaned)) {
                language.Cleaned[key] = cleaned.Replace(
                    IncorrectVariantTerm,
                    CorrectVariantTerm,
                    System.StringComparison.Ordinal
                );
            }
        }
    }

    private static bool IsExtendedVariantDialogKey(string key) {
        return key.Equals(
                ExtendedVariantModNameKey,
                System.StringComparison.OrdinalIgnoreCase
            )
            || key.StartsWith(
                ExtendedVariantOptionsKeyPrefix,
                System.StringComparison.OrdinalIgnoreCase
            );
    }

    private static void OnBirdTutorialGuiCtor(
        On.Celeste.BirdTutorialGui.orig_ctor orig,
        BirdTutorialGui self,
        Monocle.Entity entity,
        Microsoft.Xna.Framework.Vector2 position,
        object info,
        object[] controls
    ) {
        if (IsKoreanLanguage()
            && controls?.Length == 2
            && controls[0] is string holdText
            && string.Equals(
                holdText,
                Dialog.Clean("tutorial_hold"),
                System.StringComparison.Ordinal
            )
            && controls[1] is BirdTutorialGui.ButtonPrompt prompt
            && prompt == BirdTutorialGui.ButtonPrompt.Grab) {
            controls = (object[]) controls.Clone();
            (controls[0], controls[1]) = (controls[1], controls[0]);
        }

        orig(self, entity, position, info, controls);
    }

    private static bool IsKoreanLanguage() {
        return Dialog.Language?.Id?.Equals(
            KoreanLanguageId,
            System.StringComparison.OrdinalIgnoreCase
        ) == true;
    }
}
