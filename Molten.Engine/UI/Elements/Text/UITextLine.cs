﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Molten.Graphics;
using static System.Net.Mime.MediaTypeNames;

namespace Molten.UI
{
    /// <summary>
    /// Represents a line of text that is displayed by an implemented <see cref="UIElement"/>.
    /// </summary>
    public class UITextLine
    {
        public struct FindResult
        {
            /// <summary>
            /// The end-point. IF <see cref="IsReverse"/> is true, this will be the start-point instead.
            /// </summary>
            public UITextLine End;

            /// <summary>
            /// The total height of all lines in the result.
            /// </summary>
            public float Height;

            /// <summary>
            /// The largest line width found.
            /// </summary>
            public float Width;

            /// <summary>
            /// The number of lines that were found.
            /// </summary>
            public int Count;

            /// <summary>
            /// If true, <see cref="End"/> is the first line and the last line is the line that executed the find.
            /// </summary>
            public bool IsReverse;

            public FindResult(bool isReverse, UITextLine startLine)
            {
                Width = 0;
                Height = 0;
                IsReverse = isReverse;
                End = startLine;
            }
        }

        int _height;

        internal UITextLine(UITextBox element)
        {
            Parent = element;
            HasText = false;
        }

        public void Clear()
        {
            Width = 0;
            _height = 0;
            HasText = false;
            FirstSegment = new UITextSegment("", Color.White, null);
            LastSegment = FirstSegment;
        }

        internal bool Pick(ref Rectangle lBounds, ref Vector2I pickPoint, UITextCaret.CaretPoint caretPoint)
        {
            UITextSegment seg = FirstSegment;
            Vector2F fPos = (Vector2F)pickPoint;

            if (lBounds.Contains(pickPoint))
            {
                caretPoint.Line = this;
                RectangleF segBounds = (RectangleF)lBounds;

                while (seg != null)
                {
                    segBounds.Width = seg.Size.X;
                    if (segBounds.Contains(fPos))
                    {
                        caretPoint.Segment = seg;
                        SpriteFont segFont = seg.Font ?? Parent.DefaultFont;

                        if (!string.IsNullOrWhiteSpace(seg.Text))
                        {
                            float dist = 0;
                            float prevDist = 0;
                            float charDist = 0;
                            float halfDist = 0;

                            for (int i = 0; i < seg.Text.Length; i++)
                            {
                                charDist = segFont.GetAdvanceWidth(seg.Text[i]);
                                dist += charDist;
                                halfDist = prevDist + (charDist / 2);

                                if (pickPoint.X <= segBounds.Left + dist)
                                {
                                    if (pickPoint.X >= segBounds.Left + halfDist)
                                    {
                                        caretPoint.Char.Index = i+1;
                                        caretPoint.Char.StartOffset = dist;
                                        caretPoint.Char.EndOffset = segBounds.Width - dist;
                                    }
                                    else
                                    {
                                        caretPoint.Char.Index = i;
                                        caretPoint.Char.StartOffset = prevDist;
                                        caretPoint.Char.EndOffset = segBounds.Width - prevDist;
                                    }                                    
                                    break;
                                }

                                prevDist = dist;
                            }
                        } 

                        return true;
                    }
                    segBounds.X += seg.Size.X;
                    seg = seg.Next;
                }
            }

            return false;
        }

        public UITextLine Split(UITextSegment seg, int? charIndex)
        {
            // Split the text behind the caret char-index, so that it can remain on the current line.
            if (charIndex.HasValue)
            {
                string toStay = seg.Text.Substring(0, charIndex.Value);
                if (toStay.Length > 0)
                {
                    seg.Text = seg.Text.Remove(0, charIndex.Value);

                    UITextSegment newSeg = new UITextSegment(toStay, seg.Color, seg.Font);

                    if (seg.Previous != null)
                    {
                        seg.Previous.Next = newSeg;
                        newSeg.Previous = seg.Previous;
                    }

                    LastSegment = newSeg;

                    UITextLine newLine = new UITextLine(Parent);

                    seg.Previous = null;
                    newLine.AppendSegment(seg);
                    return newLine;
                }
            }

            return this;
        }

        public UITextSegment NewSegment(string text, Color color, SpriteFont font)
        {
            UITextSegment segment = new UITextSegment(text, color, font);
            AppendSegment(segment);
            return segment;
        }

        public void AppendSegment(UITextSegment seg)
        {
            if (seg != null)
            {
                if (LastSegment != null)
                {
                    LastSegment.Next = seg;
                    seg.Previous = LastSegment;
                }
                else
                {
                    LastSegment = seg;
                    FirstSegment = seg;
                }

                LastSegment = seg;
                Width += seg.Size.X;
                _height = Math.Max(_height, (int)Math.Ceiling(seg.Size.Y));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void LinkNext(UITextLine next)
        {
            Next = next;

            if (next == this)
                throw new Exception("BLEH???!??!?!");

            if (next != null)
                next.Previous = this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void LinkPrevious(UITextLine prev)
        {
            Previous = prev;

            if (Previous == this)
                throw new Exception("BLEH???!??!?!");
            if (prev != null)
                prev.Next = this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void UnlinkNext()
        {
            if(Next != null)
            {
                Next.Previous = null;
                Next = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void UnlinkPrevious()
        {
            if (Previous != null)
            {
                Previous.Next = null;
                Previous = null;
            }
        }

        /// <summary>
        /// Iterates over a chain of <see cref="UITextLine"/>, starting with the current <see cref="UITextLine"/> until the last one is found.
        /// </summary>
        /// <param name="count">Output for number of <see cref="UITextLine"/> found between the current and last <see cref="UITextLine"/>.</param>
        /// <returns></returns>
        internal UITextLine FindLast(out int count)
        {
            UITextLine line = this;
            UITextLine last = line;
            count = 0;

            while (line != null)
            {
                count++;

                last = line;
                line = line.Next;
            }

            return last;
        }

        /// <summary>
        /// Iterates over a chain of lines, starting with the current <see cref="UITextLine"/> until the <paramref name="end"/> <see cref="UITextLine"/> is found. 
        /// <para>Returns 0 if <paramref name="end"/> is not found.</para>
        /// </summary>
        /// <param name="end">The end <see cref="UITextLine"/>.</param>
        /// <returns>The number of lines between the current and <paramref name="end"/>.</returns>
        internal FindResult FindUntil(UITextLine end)
        {
            FindResult result = new FindResult(false, this);
            UITextLine line = this;
            result.End = end;

            while (line != null)
            {
                result.Width = float.Max(result.Width, line.Width);
                result.Height += line.Height;
                result.Count++;

                if (line == end)
                    break;

                if (line.Next == null)
                {
                    result.End = line;
                    break;
                }

                line = line.Next;
            }

            return result;
        }

        /// <summary>
        /// Iterates over a chain of lines, starting with the current <see cref="UITextLine"/> until the the <paramref name="maxCount"/> is found, the last line is found.
        /// </summary>
        /// <param name="maxCount">The max number of lines to find.</param>
        /// <returns>A <see cref="FindResult"/></returns>
        internal FindResult FindUntil(int maxCount)
        {
            FindResult result = new FindResult(false, this);
            UITextLine line = this;

            while (line != null)
            {
                result.Width = float.Max(result.Width, line.Width);
                result.Height += line.Height;
                result.Count++;

                if (result.Count == maxCount)
                    break;

                result.End = line;
                line = line.Next;
            }

            return result;
        }

        /// <summary>
        /// Iterates over a chain of lines, starting with the current <see cref="UITextLine"/> until the the <paramref name="maxCount"/> is found, the first line is found.
        /// </summary>
        /// <param name="maxCount">The max number of lines to find.</param>
        /// <returns>The last line that was found.</returns>
        internal FindResult FindUntilReverse(int maxCount)
        {
            FindResult result = new FindResult(true, this);
            UITextLine line = this;

            while (line != null)
            {
                result.Width = float.Max(result.Width, line.Width);
                result.Height += line.Height;
                result.Count++;

                if (result.Count == maxCount)
                    break;

                result.End = line;
                line = line.Previous;
            }

            return result;
        }

        /// <summary>
        /// Creates and inserts a new <see cref="UITextSegment"/> after the specified <see cref="UITextSegment"/>.
        /// </summary>
        /// <param name="after">The <see cref="UITextSegment"/> that the new segment should be inserted after.
        /// <para>If null, the new segment will be insert at the beginning of the line.</para></param>
        /// <param name="color">The color of the new segment's text.</param>
        /// <param name="font">The font of the new segment.</param>
        /// <returns></returns>
        public UITextSegment InsertSegment(UITextSegment after, Color color, SpriteFont font)
        {
            UITextSegment next = new UITextSegment("", Color.White, font);

            // Do we need to insert before another "next" segment also?
            if (after != null)
            {
                if (after.Next != null)
                {
                    after.Next.Previous = next;
                    next.Next = after.Next;
                }

                after.Next = next;
                next.Previous = after;
            }
            else
            {
                if(FirstSegment != null)
                {
                    FirstSegment.Previous = after;
                    after.Next = FirstSegment;
                    FirstSegment = after;
                }
                else
                {
                    FirstSegment = after;
                    LastSegment = after;
                }
            }

            Width += after.Size.X;
            _height = Math.Max(_height, (int)Math.Ceiling(after.Size.Y));

            return next;
        }

        /// <summary>
        /// Gets the string of text represented by all <see cref="UITextSegment"/>s of the current <see cref="UITextLine"/>.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            string result = "";
            UITextSegment seg = FirstSegment;
            while(seg != null)
            {
                result += seg.Text;
                seg = seg.Next;
            }

            return result;
        }

        /// <summary>
        /// Gets the string of text represented by all <see cref="UITextSegment"/>s of the current <see cref="UITextLine"/> and appends it to the provided <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to output the line contents into.</param>
        /// <returns></returns>
        public void GetText(StringBuilder sb)
        {
            UITextSegment seg = FirstSegment;
            while (seg != null)
            {
                sb.Append(seg.Text);
                seg = seg.Next;
            }
        }

        /// <summary>
        /// Gets the measured width of the current <see cref="UITextLine"/>.
        /// </summary>
        public float Width { get; private set; }

        /// <summary>
        /// Gets the measured height of the current <see cref="UITextLine"/>.
        /// <para>If the line contains no text, the default line height of the <see cref="Parent"/> will be used instead.</para>
        /// </summary>
        public int Height => HasText ? _height : Parent.DefaultLineHeight;

        /// <summary>
        /// Gets the parent <see cref="UITextBox"/>, or null if none.
        /// </summary>
        public UITextBox Parent { get; internal set; }

        /// <summary>
        /// Gets the first <see cref="UITextSegment"/> on the current <see cref="UITextLine"/>.
        /// </summary>
        public UITextSegment FirstSegment { get; private set; }

        /// <summary>
        /// Gets the last <see cref="UITextSegment"/> on the current <see cref="UITextLine"/>.
        /// </summary>
        public UITextSegment LastSegment { get; private set; }

        public UITextLine Previous { get; internal set; }

        public UITextLine Next { get; internal set; }

        /// <summary>
        /// Gets whether or not the current <see cref="UITextLine"/> contains text.
        /// </summary>
        public bool HasText { get; private set; }
    }
}
