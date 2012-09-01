using Caliburn.Micro;
using VChat.Models;

namespace VChat.ViewModels.Data
{
    public class AttachmentViewModel : PropertyChangedBase
    {
        public static AttachmentViewModel Map(MessageAttachment attachment)
        {
            if (attachment is MessagePhotoAttachment)
                return PhotoViewModel.Map(((MessagePhotoAttachment)attachment).Photo);

            if (attachment is MessageVideoAttachment)
                return VideoViewModel.Map(((MessageVideoAttachment)attachment).Video);

            if (attachment is MessageAudioAttachment)
                return AudioViewModel.Map(((MessageAudioAttachment)attachment).Audio);

            if (attachment is MessageDocumentAttachment)
                return DocumentViewModel.Map(((MessageDocumentAttachment)attachment).Document);

            if (attachment is MessageWallAttachment)
                return WallViewModel.Map(((MessageWallAttachment)attachment).Wall);

            return new AttachmentViewModel();
        }

        public static AttachmentViewModel Map(WallAttachment attachment)
        {
            if (attachment is WallPhotoAttachment)
                return PhotoViewModel.Map(((WallPhotoAttachment)attachment).Photo);

            if (attachment is WallVideoAttachment)
                return VideoViewModel.Map(((WallVideoAttachment)attachment).Video);

            if (attachment is WallAudioAttachment)
                return AudioViewModel.Map(((WallAudioAttachment)attachment).Audio);

            if (attachment is WallDocumentAttachment)
                return DocumentViewModel.Map(((WallDocumentAttachment)attachment).Document);

            if (attachment is WallWallAttachment)
                return WallViewModel.Map(((WallWallAttachment)attachment).WallWall);

            return new AttachmentViewModel();
        }
    }
}